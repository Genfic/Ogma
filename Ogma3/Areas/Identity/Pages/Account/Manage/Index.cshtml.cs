using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using B2Net;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Configuration;
using Ogma3.Data;
using Ogma3.Data.Models;
using Ogma3.Services.Attributes;

namespace Ogma3.Areas.Identity.Pages.Account.Manage
{
    public partial class IndexModel : PageModel
    {
        private readonly OgmaUserManager _userManager;
        private readonly SignInManager<Data.Models.User> _signInManager;
        private readonly IB2Client _b2Client;
        private readonly IConfiguration _config;

        public IndexModel(
            OgmaUserManager userManager,
            SignInManager<Data.Models.User> signInManager,
            IB2Client b2Client,
            IConfiguration config)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _b2Client = b2Client;
            _config = config;
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
            [MaxFileSize(CTConfig.Files.AvatarMaxWeight)]
            [AllowedExtensions(new[] { ".jpg", ".jpeg", ".png" })]
            public IFormFile Avatar { get; set; }
            
            [Display(Name = "Title")]
            [StringLength(CTConfig.User.MaxTitleLength, ErrorMessage = "The {0} must be no longer than {1} characters long.")]
            public string Title { get; set; }
            
            [Display(Name = "Bio")]
            [StringLength(CTConfig.User.MaxBioLength, ErrorMessage = "The {0} must be no longer than {1} characters long.")]
            public string Bio { get; set; }
        }

        private async Task LoadAsync(Data.Models.User user)
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
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            if (!ModelState.IsValid)
            {
                await LoadAsync(user);
                return Page();
            }
            
            // Handle avatar upload
            if (Input.Avatar != null && Input.Avatar.Length > 0)
            {
                var ext = Input.Avatar.FileName.Split('.').Last();
                var fileName = $"avatars/{user.Id}.{ext}";
                
                // Delete the old one if exists
                if (user.Avatar != null && user.AvatarId != null)
                {
                    await _b2Client.Files.Delete(user.AvatarId, user.Avatar.Replace(_config["cdn"], ""));
                }
                
                // Upload new one
                await using var ms = new MemoryStream();
                Input.Avatar.CopyTo(ms);
                var file = await _b2Client.Files.Upload(ms.ToArray(), fileName);

                user.AvatarId = file.FileId;
                user.Avatar = _config["cdn"] + fileName;
            }

            if (Input.Title != user.Title)
            {
                user.Title = Input.Title;
            }

            if (Input.Bio != user.Bio)
            {
                user.Bio = Input.Bio;
            }

            var update = await _userManager.UpdateAsync(user);

            await _signInManager.RefreshSignInAsync(user);
            StatusMessage = "Your profile has been updated";
            return RedirectToPage();
        }
    }
}
