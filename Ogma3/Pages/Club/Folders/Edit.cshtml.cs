using System.Linq;
using System.Threading.Tasks;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Ogma3.Data;
using Ogma3.Data.Clubs;
using Ogma3.Infrastructure.Extensions;
using Utils.Extensions;

namespace Ogma3.Pages.Club.Folders;

public class EditModel : PageModel
{
    private readonly ApplicationDbContext _context;
    private readonly ClubRepository _clubRepo;

    public EditModel(ApplicationDbContext context, ClubRepository clubRepo)
    {
        _context = context;
        _clubRepo = clubRepo;
    }
        
    public async Task<IActionResult> OnGet(long clubId, long id)
    {
        var uid = User.GetNumericId();
        if (uid is null) return Unauthorized();
            
        // Check if founder or admin
        var isFounder = await _clubRepo.CheckRoles(clubId, (long) uid, new[]{ EClubMemberRoles.Founder, EClubMemberRoles.Admin });
        if (!isFounder) return Unauthorized();

        Input = await _context.Folders
            .Where(f => f.ClubId == clubId)
            .Where(f => f.Id == id)
            .Select(f => new InputModel
            {
                Id = f.Id,
                ClubId = f.ClubId,
                Name = f.Name,
                Description = f.Description,
                ParentId = f.ParentFolderId,
                Role = f.AccessLevel
            })
            .FirstOrDefaultAsync();
            
        if (Input is null) return NotFound();

        return Page();
    }

    [BindProperty]
    public InputModel Input { get; set; }
        
    public class InputModel
    {
        public long Id { get; init; }
        public long ClubId { get; init; }
        public string Name { get; init; }
        public string Description { get; init; }
        public long? ParentId { get; init; }
        public EClubMemberRoles Role { get; init; }
    }
        
    public class PostDataValidation : AbstractValidator<InputModel>
    {
        public PostDataValidation()
        {
            RuleFor(b => b.Name)
                .NotEmpty()
                .Length(CTConfig.CFolder.MinNameLength, CTConfig.CFolder.MaxNameLength);
            RuleFor(b => b.Description)
                .MaximumLength(CTConfig.CFolder.MaxDescriptionLength);
        }
    }

    public async Task<IActionResult> OnPostAsync(long clubId, long id)
    {
        if (!ModelState.IsValid) return Page();// await OnGet(clubId, id);
            
        var uid = User.GetNumericId();
        if (uid is null) return Unauthorized();
            
        // Check if authorized
        var isAuthorized = await _clubRepo.CheckRoles(clubId, (long) uid, new[]{ EClubMemberRoles.Founder, EClubMemberRoles.Admin });
        if (!isAuthorized) return Unauthorized();
            
        var folder = await _context.Folders
            .Where(f => f.ClubId == clubId)
            .Where(f => f.Id == Input.Id)
            .Include(f => f.ChildFolders)
            .FirstOrDefaultAsync();

        if (folder is null) return NotFound();

        folder.Name = Input.Name;
        folder.Slug = Input.Name.Friendlify();
        folder.AccessLevel = Input.Role;
        folder.Description = Input.Description;
            
        // Prevent nesting the parent folder inside of one of its children or inside of itself
        if (folder.ChildFolders.Any(cf => cf.Id == Input.ParentId))
        {
            ModelState.AddModelError("", "The folder cannot be nested inside of its child");
            return Page();
        }
        if (folder.Id == Input.ParentId)
        {
            ModelState.AddModelError("", "The folder cannot be nested inside of itself");
            return Page();
        }

        folder.ParentFolderId = Input.ParentId;

        await _context.SaveChangesAsync();
            
        return RedirectToPage("./Folder", new { clubId, folder.Id, folder.Slug });
    }
}