using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Ogma3.Data.Users;

namespace Ogma3.Areas.Identity.Pages.Account.Manage;

[Authorize]
public class ChangePasswordModel(
	UserManager<OgmaUser> userManager,
	SignInManager<OgmaUser> signInManager,
	ILogger<ChangePasswordModel> logger)
	: PageModel
{
	
	[BindProperty] public required InputModel Input { get; set; }

	[TempData] public required string StatusMessage { get; set; }

	public class InputModel
	{
		[Required]
		[DataType(DataType.Password)]
		[Display(Name = "Current password")]
		public required string OldPassword { get; init; }

		[Required]
		[StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
		[DataType(DataType.Password)]
		[Display(Name = "New password")]
		public required string NewPassword { get; init; }

		[DataType(DataType.Password)]
		[Display(Name = "Confirm new password")]
		[Compare("NewPassword", ErrorMessage = "The new password and confirmation password do not match.")]
		public required string ConfirmPassword { get; init; }
	}

	public async Task<IActionResult> OnGetAsync()
	{
		var user = await userManager.GetUserAsync(User);
		if (user is null)
		{
			return NotFound($"Unable to load user with ID '{userManager.GetUserId(User)}'.");
		}

		var hasPassword = await userManager.HasPasswordAsync(user);
		if (!hasPassword)
		{
			return RedirectToPage("./SetPassword");
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

		var changePasswordResult = await userManager.ChangePasswordAsync(user, Input.OldPassword, Input.NewPassword);
		if (!changePasswordResult.Succeeded)
		{
			foreach (var error in changePasswordResult.Errors)
			{
				ModelState.AddModelError(string.Empty, error.Description);
			}

			return Page();
		}

		await signInManager.RefreshSignInAsync(user);
		logger.LogInformation("User changed their password successfully");
		StatusMessage = "Your password has been changed.";

		return RedirectToPage();
	}
}