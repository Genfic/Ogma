using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Ogma3.Data;
using Ogma3.Data.Enums;
using Ogma3.Data.Models;
using Ogma3.Data.Repositories;
using Utils.Extensions;

namespace Ogma3.Pages.Club.Folders
{
    public class CreateModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly ClubRepository _clubRepo;

        public CreateModel(ApplicationDbContext context, ClubRepository clubRepo)
        {
            _context = context;
            _clubRepo = clubRepo;
        }

        public long ClubId { get; set; }
        
        public async Task<IActionResult> OnGet(long clubId)
        {
            ClubId = clubId;
            
            var uid = User.GetNumericId();
            if (uid == null) return Unauthorized();
            
            // Check if founder
            var isFounder = await _clubRepo.CheckRoles(clubId, (long) uid, new[]{EClubMemberRoles.Founder, EClubMemberRoles.Admin});
            if (!isFounder) return Unauthorized();
            
            return Page();
        }

        [BindProperty]
        public InputModel Input { get; set; }
        
        public class InputModel
        {
            [Required]
            [MinLength(CTConfig.CFolder.MinNameLength)]
            [MaxLength(CTConfig.CFolder.MaxNameLength)]
            public string Name { get; set; }
            
            [MaxLength(CTConfig.CFolder.MaxDescriptionLength)]
            public string Description { get; set; }

            public long? ParentId { get; set; }
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
            
            var folder = new Folder
            {
                Name = Input.Name,
                Slug = Input.Name.Friendlify(),
                Description = Input.Description,
                ClubId = clubId,
                ParentFolderId = Input.ParentId
            };

            await _context.Folders.AddAsync(folder);
            await _context.SaveChangesAsync();
            
            return RedirectToPage("./Index", new { clubId });
        }
    }
}