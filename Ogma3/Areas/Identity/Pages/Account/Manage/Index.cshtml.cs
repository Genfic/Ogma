using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Ogma3.Data;

namespace Ogma3.Areas.Identity.Pages.Account.Manage
{
    public partial class IndexModel : PageModel
    {
        private readonly OgmaUserManager _userManager;
        private readonly SignInManager<User> _signInManager;

        public IndexModel(
            OgmaUserManager userManager,
            SignInManager<User> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        public string Username { get; set; }

        [TempData]
        public string StatusMessage { get; set; }

        [BindProperty]
        public InputModel Input { get; set; }

        public class InputModel
        {
            [Display(Name = "Title")]
            [StringLength(20, ErrorMessage = "The {0} must be no longer than {1} characters long.")]
            public string Title { get; set; }
            
            [Display(Name = "Bio")]
            [StringLength(2000, ErrorMessage = "The {0} must be no longer than {1} characters long.")]
            public string Bio { get; set; }
        }

        private async Task LoadAsync(User user)
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
