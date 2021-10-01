using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Ogma3.Data;
using Ogma3.Data.Clubs;
using Ogma3.Infrastructure.Extensions;

namespace Ogma3.Pages.Club.Folders;

public class DeleteModel : PageModel
{
    private readonly ApplicationDbContext _context;
    private readonly ClubRepository _clubRepo;

    public DeleteModel(ApplicationDbContext context, ClubRepository clubRepo)
    {
        _context = context;
        _clubRepo = clubRepo;
    }

        
    [BindProperty] 
    public DeleteViewModel Folder { get; set; }
        
    public class DeleteViewModel
    {
        public string Name { get; init; }
        public string Description { get; init; }
        public int StoriesCount { get; init; }
        public long ClubId { get; init; }
        public long Id { get; init; }
    }
        
    public async Task<IActionResult> OnGet(long clubId, long id)
    {
        var uid = User.GetNumericId();
        if (uid is null) return Unauthorized();
            
        // Check if authorized
        var isAuthorized = await _clubRepo.CheckRoles(clubId, (long) uid, new[]{EClubMemberRoles.Founder, EClubMemberRoles.Admin});
        if (!isAuthorized) return Unauthorized();

        Folder = await _context.Folders
            .Where(f => f.ClubId == clubId)
            .Where(f => f.Id == id)
            .Select(f => new DeleteViewModel
            {
                Id = f.Id,
                ClubId = f.ClubId,
                Name = f.Name,
                Description = f.Description,
                StoriesCount = f.StoriesCount
            })
            .AsNoTracking()
            .FirstOrDefaultAsync();
            
        if (Folder == null) return NotFound();

        return Page();
    }

    public async Task<IActionResult> OnPostAsync(long clubId, long? id)
    {
        var uid = User.GetNumericId();
        if (uid == null) return Unauthorized();
            
        // Check if authorized
        var isAuthorized = await _clubRepo.CheckRoles(clubId, (long) uid, new[]{EClubMemberRoles.Founder, EClubMemberRoles.Admin});
        if (!isAuthorized) return Unauthorized();
            
        var folder = await _context.Folders
            .Where(f => f.ClubId == clubId)
            .Where(f => f.Id == id)
            .Include(f => f.ChildFolders)
            .FirstOrDefaultAsync();

        if (folder is null) return NotFound();
            
        // Prevent deleting a folder that has children
        if (folder.ChildFolders.Count > 0)
        {
            ModelState.AddModelError("asd", "You cannot delete a folder that has children, delete the children first");
            return Page();
        }

        _context.Folders.Remove(folder);
        await _context.SaveChangesAsync();
            
        return RedirectToPage("./Index", new { id = clubId });
    }
}