using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using B2Net;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Ogma3.Data;
using Ogma3.Data.Models;
using Ogma3.Infrastructure.Attributes;
using Ogma3.Services.FileUploader;

namespace Ogma3.Areas.Identity.Pages.Account.Manage
{
    public class IndexModel : PageModel
    {
        private readonly OgmaUserManager _userManager;
        private readonly SignInManager<OgmaUser> _signInManager;
        private readonly IB2Client _b2Client;
        private readonly FileUploader _uploader;
        private readonly OgmaConfig _ogmaConfig;

        public IndexModel(
            OgmaUserManager userManager,
            SignInManager<OgmaUser> signInManager,
            IB2Client b2Client,
            FileUploader uploader, 
            OgmaConfig ogmaConfig)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _b2Client = b2Client;
            _uploader = uploader;
            _ogmaConfig = ogmaConfig;
        }

        
        public string Username { get; set; }

        [TempData]
        public string StatusMessage { get; set; }

        [BindProperty]
        public InputModel Input { get; set; }

        
        public class InputModel
        {
            [Display(Name = "Avatar")]
            [DataType(DataType.Upload)]
            [MaxFileSize(CTConfig.CFiles.AvatarMaxWeight)]
            [AllowedExtensions(new[] { ".jpg", ".jpeg", ".png" })]
            public IFormFile Avatar { get; set; }
            
            [Display(Name = "Title")]
            [StringLength(CTConfig.CUser.MaxTitleLength, ErrorMessage = "The {0} must be no longer than {1} characters long.")]
            public string Title { get; set; }
            
            [Display(Name = "Bio")]
            [StringLength(CTConfig.CUser.MaxBioLength, ErrorMessage = "The {0} must be no longer than {1} characters long.")]
            public string Bio { get; set; }
        }

        private async Task LoadAsync(OgmaUser user)
        {
            var userName = await _userManager.GetUserNameAsync(user);
            var userTitle = await _userManager.GetTitleAsync(user);
            var userBio = await _userManager.GetBioAsync(user);
            
            Username = userName;

            Input = new InputModel
            {
                Title = userTitle,
                Bio = userBio,
            };
        }

        public async Task<IActionResult> OnGetAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            await LoadAsync(user);
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound("Unable to load user");
            }

            if (!ModelState.IsValid)
            {
                await LoadAsync(user);
                return Page();
            }

            // If new avatar provided, replace the old one
            if (Input.Avatar != null && Input.Avatar.Length > 0)
            {
                // Delete the old avatar if exists
                if (user.Avatar != null && user.AvatarId != null)
                {
                    await _b2Client.Files.Delete(user.AvatarId, user.Avatar.Replace(_ogmaConfig.Cdn, ""));
                }

                // Upload the new one
                var file = await _uploader.Upload(
                    Input.Avatar, 
                    "avatars", 
                    $"U-{user.NormalizedUserName}",
                    _ogmaConfig.AvatarWidth,
                    _ogmaConfig.AvatarHeight
                );
                user.AvatarId = file.FileId;
                user.Avatar = file.Path;
            }

            if (Input.Title != user.Title)
            {
                user.Title = Input.Title;
            }

            if (Input.Bio != user.Bio)
            {
                user.Bio = Input.Bio;
            }

            await _userManager.UpdateAsync(user);

            await _signInManager.RefreshSignInAsync(user);
            StatusMessage = "Your profile has been updated";
            return RedirectToPage();
        }
    }
}
