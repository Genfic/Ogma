using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Ogma3.Data;
using Ogma3.Data.Enums;
using Ogma3.Data.Models;
using Ogma3.Infrastructure.Attributes;
using Ogma3.Services.FileUploader;
using Utils.Extensions;

namespace Ogma3.Pages.Clubs
{
    public class CreateModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly OgmaUserManager _userManager;
        private readonly FileUploader _uploader;
        private readonly OgmaConfig _config;

        public CreateModel(ApplicationDbContext context, OgmaUserManager userManager, FileUploader uploader, OgmaConfig config)
        {
            _context = context;
            _userManager = userManager;
            _uploader = uploader;
            _config = config;
        }

        public IActionResult OnGet()
        {
            return Page();
        }

        [BindProperty]
        public InputModel Input { get; set; }
        
        public class InputModel
        {
                [Required]
                [MinLength(CTConfig.CClub.MinNameLength)]
                [MaxLength(CTConfig.CClub.MaxNameLength)]
                public string Name { get; set; }
                
                [MaxLength(CTConfig.CClub.MaxHookLength)]
                public string Hook { get; set; }
                
                [MaxLength(CTConfig.CClub.MaxDescriptionLength)]
                public string Description { get; set; }
                
                [DataType(DataType.Upload)]
                [MaxFileSize(CTConfig.CStory.CoverMaxWeight)]
                [AllowedExtensions(new[] {".jpg", ".jpeg", ".png"})]
                public IFormFile Icon { get; set; }
        }

        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var currentUser = await _userManager.GetUserAsync(User);
            
            var clubMember = new ClubMember
            {
                Member = currentUser,
                Role = EClubMemberRoles.Founder,
                MemberSince = DateTime.Now
            };

            var club = new Data.Models.Club
            {
                Name = Input.Name,
                Slug = Input.Name.Friendlify(),
                Hook = Input.Hook,
                Description = Input.Description,
                CreationDate = DateTime.Now,
                ClubMembers = new List<ClubMember> { clubMember }
            };

            await _context.Clubs.AddAsync(club);
            await _context.SaveChangesAsync();
            
            if (Input.Icon != null && Input.Icon.Length > 0)
            {
                var file = await _uploader.Upload(
                    Input.Icon, 
                    "club-icons", 
                    $"{club.Id}-{club.Name.Friendlify()}",
                    _config.ClubIconWidth,
                    _config.ClubIconHeight
                );
                club.IconId = file.FileId;
                club.Icon = file.Path;
                // Final save
                await _context.SaveChangesAsync();
            }

            return RedirectToPage("./Index");
        }
    }
}
