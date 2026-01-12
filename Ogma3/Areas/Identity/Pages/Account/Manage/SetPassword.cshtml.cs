using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Ogma3.Data;
using Ogma3.Data.Users;
using Routes.Areas.Identity.Pages;

namespace Ogma3.Areas.Identity.Pages.Account.Manage;

public sealed class SetPasswordModel
(
	UserManager<OgmaUser> userManager,
	SignInManager<OgmaUser> signInManager)
	: PageModel
{

	[BindProperty] public required InputModel Input { get; set; }

	[TempData] public required string StatusMessage { get; set; }

	public sealed class InputModel
	{
		[Required]
		[StringLength(
			CTConfig.User.MaxPassLength,
			ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.",
			MinimumLength = CTConfig.User.MinPassLength
		)]
		[DataType(DataType.Password)]
		[Display(Name = "New password")]
		public required string NewPassword { get; set; }

		[DataType(DataType.Password)]
		[Display(Name = "Confirm new password")]
		[Compare("NewPassword", ErrorMessage = "The new password and confirmation password do not match.")]
		public required string ConfirmPassword { get; set; }
	}

	public async Task<IActionResult> OnGetAsync()
	{
		var user = await userManager.GetUserAsync(User);
		if (user is null)
		{
			return NotFound($"Unable to load user with ID '{userManager.GetUserId(User)}'.");
		}

		var hasPassword = await userManager.HasPasswordAsync(user);

		if (hasPassword)
		{
			return Account_Manage_ChangePassword.Get().Redirect(this);
		}

		return Page();
	}

	public async Task<IActionResult> OnPostAsync()
	{
		if (!ModelState.IsValid)
		{
			return Page();
		}

		var user = await userManager.GetUserAsync(User);
		if (user is null)
		{
			return NotFound($"Unable to load user with ID '{userManager.GetUserId(User)}'.");
		}

		var addPasswordResult = await userManager.AddPasswordAsync(user, Input.NewPassword);
		if (!addPasswordResult.Succeeded)
		{
			foreach (var error in addPasswordResult.Errors)
			{
				ModelState.AddModelError(string.Empty, error.Description);
			}

			return Page();
		}

		await signInManager.RefreshSignInAsync(user);
		StatusMessage = "Your password has been set.";

		return RedirectToPage();
	}
}