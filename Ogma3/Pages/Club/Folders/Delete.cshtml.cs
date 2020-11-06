using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Ogma3.Data;
using Ogma3.Data.Enums;
using Ogma3.Data.Models;
using Ogma3.Data.Repositories;
using Utils.Extensions;

namespace Ogma3.Pages.Club.Folders
{
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
        public Folder Folder { get; set; }
        
        public async Task<IActionResult> OnGet(long clubId, long id)
        {
            var uid = User.GetNumericId();
            if (uid == null) return Unauthorized();
            
            // Check if founder
            var isFounder = await _clubRepo.CheckRoles(clubId, (long) uid, new[]{EClubMemberRoles.Founder, EClubMemberRoles.Admin});
            if (!isFounder) return Unauthorized();

            Folder = await _context.Folders
                .Where(f => f.ClubId == clubId)
                .Where(f => f.Id == id)
                .AsNoTracking()
                .FirstOrDefaultAsync();
            
            if (Folder == null) return NotFound();

            return Page();
        }

        public async Task<IActionResult> OnPostAsync(long clubId, long? id)
        {
            var uid = User.GetNumericId();
            if (uid == null) return Unauthorized();
            
            // Check if founder
            var isFounder = await _clubRepo.CheckRoles(clubId, (long) uid, new[]{EClubMemberRoles.Founder, EClubMemberRoles.Admin});
            if (!isFounder) return Unauthorized();
            
            Folder = await _context.Folders
                .Where(f => f.ClubId == clubId)
                .Where(f => f.Id == id)
                .Include(f => f.ChildFolders)
                .FirstOrDefaultAsync();

            if (Folder == null) return NotFound();
            
            // Prevent deleting a folder that has children
            if (Folder.ChildFolders.Count > 0)
            {
                ModelState.AddModelError("asd", "You cannot delete a folder that has children, delete the children first");
                return Page();
            }

            _context.Folders.Remove(Folder);
            await _context.SaveChangesAsync();
            
            return RedirectToPage("./Index", new { clubId });
        }
    }
}