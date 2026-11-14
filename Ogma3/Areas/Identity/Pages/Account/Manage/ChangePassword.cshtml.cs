using System.ComponentModel.DataAnnotations;
using Ogma3.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Ogma3.Data.SecurityTransactions;
using Ogma3.Data.Users;
using Ogma3.Services.Security;

namespace Ogma3.Areas.Identity.Pages.Account.Manage;

[Authorize]
public sealed class ChangePasswordModel(
	OgmaUserManager userManager,
	SignInManager<OgmaUser> signInManager,
	SecurityAuthorizationService securityAuth,
	SecurityNotificationService notifications,
	ILogger<ChangePasswordModel> logger)
	: PageModel
{

	[BindProperty] public required InputModel Input { get; set; }

	[TempData] public required string StatusMessage { get; set; }

	public sealed class InputModel
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
			return Routes.Areas.Identity.Pages.Account_Manage_SetPassword.Get().Redirect(this);
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

		var hasPassword = await userManager.HasPasswordAsync(user);
		if (!hasPassword)
		{
			return Routes.Areas.Identity.Pages.Account_Manage_SetPassword.Get().Redirect(this);
		}

		// Changing the password requires fresh account proof
		if (!await securityAuth.HasVerifiedGrantAsync(user, SecurityOperation.ChangePassword))
		{
			var txn = await securityAuth.BeginAsync(user, SecurityOperation.ChangePassword);
			return ReauthRequired.RedirectToReauth(txn, CurrentPath(), this);
		}

		// Verify the current password before consuming the grant so a typo doesn't burn it
		if (!await userManager.CheckPasswordAsync(user, Input.OldPassword))
		{
			ModelState.AddModelError(string.Empty, "Incorrect current password.");
			return Page();
		}

		await using var dbTx = await securityAuth.BeginCredentialMutationAsync(user.Id);

		if (!await securityAuth.ConsumeAsync(user, SecurityOperation.ChangePassword))
		{
			await dbTx.RollbackAsync();
			var txn = await securityAuth.BeginAsync(user, SecurityOperation.ChangePassword);
			return ReauthRequired.RedirectToReauth(txn, CurrentPath(), this);
		}

		var changePasswordResult = await userManager.ChangePasswordAsync(user, Input.OldPassword, Input.NewPassword);
		if (!changePasswordResult.Succeeded)
		{
			await dbTx.RollbackAsync();
			foreach (var error in changePasswordResult.Errors)
			{
				ModelState.AddModelError(string.Empty, error.Description);
			}

			return Page();
		}

		await securityAuth.InvalidateForUserAsync(user.Id);
		await dbTx.CommitAsync();

		// ChangePasswordAsync already bumps the security stamp; renew the current session's cookie
		await signInManager.RefreshSignInAsync(user);

		logger.LogInformation("User changed their password successfully");
		await notifications.SendAsync(user, "Your password was changed",
			"Your Genfic account password was just changed. If this wasn't you, reset your password immediately from the login page.");

		StatusMessage = "Your password has been changed.";

		return RedirectToPage();
	}

	private string CurrentPath() => Request.Path.ToString();
}
