using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Ogma3.Data;
using Ogma3.Data.Enums;
using Ogma3.Data.Repositories;
using Ogma3.Infrastructure.Extensions;
using Utils.Extensions;

namespace Ogma3.Pages.Club.Folders
{
    public class EditModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly ClubRepository _clubRepo;

        public EditModel(ApplicationDbContext context, ClubRepository clubRepo)
        {
            _context = context;
            _clubRepo = clubRepo;
        }

        public long ClubId { get; set; }
        
        public async Task<IActionResult> OnGet(long clubId, long id)
        {
            ClubId = clubId;
            
            var uid = User.GetNumericId();
            if (uid == null) return Unauthorized();
            
            // Check if founder or admin
            var isFounder = await _clubRepo.CheckRoles(clubId, (long) uid, new[]{EClubMemberRoles.Founder, EClubMemberRoles.Admin});
            if (!isFounder) return Unauthorized();

            var folder = await _context.Folders
                .Where(f => f.ClubId == clubId)
                .Where(f => f.Id == id)
                .AsNoTracking()
                .FirstOrDefaultAsync();
            
            if (folder == null) return NotFound();
            
            Input = new InputModel
            {
                Id = folder.Id,
                Name = folder.Name,
                Description = folder.Description,
                ParentId = folder.ParentFolderId,
                Role = folder.AccessLevel
            };

            return Page();
        }

        [BindProperty]
        public InputModel Input { get; set; }
        
        public class InputModel
        {
            [Required]
            public long Id { get; set; }
            
            [Required]
            [MinLength(CTConfig.CFolder.MinNameLength)]
            [MaxLength(CTConfig.CFolder.MaxNameLength)]
            public string Name { get; set; }
            
            [MaxLength(CTConfig.CFolder.MaxDescriptionLength)]
            public string Description { get; set; }

            public long? ParentId { get; set; }
            public EClubMemberRoles Role { get; set; }
        }

        public async Task<IActionResult> OnPostAsync(long clubId)
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var uid = User.GetNumericId();
            if (uid == null) return Unauthorized();
            
            // Check if founder
            var isFounder = await _clubRepo.CheckRoles(clubId, (long) uid, new[]{EClubMemberRoles.Founder, EClubMemberRoles.Admin});
            if (!isFounder) return Unauthorized();
            
            var folder = await _context.Folders
                .Where(f => f.ClubId == clubId)
                .Where(f => f.Id == Input.Id)
                .Include(f => f.ChildFolders)
                .FirstOrDefaultAsync();

            if (folder == null) return NotFound();

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
}