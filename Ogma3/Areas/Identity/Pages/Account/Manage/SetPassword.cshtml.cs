using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Ogma3.Data;
using Ogma3.Data.SecurityTransactions;
using Ogma3.Data.Users;
using Ogma3.Services.Security;
using Routes.Areas.Identity.Pages;

namespace Ogma3.Areas.Identity.Pages.Account.Manage;

public sealed class SetPasswordModel
(
	OgmaUserManager userManager,
	SignInManager<OgmaUser> signInManager,
	SecurityAuthorizationService securityAuth,
	SecurityNotificationService notifications)
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

		if (await userManager.HasPasswordAsync(user))
		{
			return Account_Manage_ChangePassword.Get().Redirect(this);
		}

		// Adding a password requires fresh account proof — a passkey, or external provider
		// re-verification for accounts without any passkey
		if (!await securityAuth.HasVerifiedGrantAsync(user, SecurityOperation.AddPassword))
		{
			var txn = await securityAuth.BeginAsync(user, SecurityOperation.AddPassword);
			return ReauthRequired.RedirectToReauth(txn, Request.Path.ToString(), this);
		}

		await using var dbTx = await securityAuth.BeginCredentialMutationAsync(user.Id);

		if (!await securityAuth.ConsumeAsync(user, SecurityOperation.AddPassword))
		{
			await dbTx.RollbackAsync();
			var txn = await securityAuth.BeginAsync(user, SecurityOperation.AddPassword);
			return ReauthRequired.RedirectToReauth(txn, Request.Path.ToString(), this);
		}

		var addPasswordResult = await userManager.AddPasswordAsync(user, Input.NewPassword);
		if (!addPasswordResult.Succeeded)
		{
			await dbTx.RollbackAsync();
			foreach (var error in addPasswordResult.Errors)
			{
				ModelState.AddModelError(string.Empty, error.Description);
			}

			return Page();
		}

		await securityAuth.InvalidateForUserAsync(user.Id);
		await dbTx.CommitAsync();

		// AddPasswordAsync bumps the security stamp; renew the current session's cookie
		await signInManager.RefreshSignInAsync(user);

		await notifications.SendAsync(user, "Password sign-in was added to your account",
			"Password sign-in was just added to your Genfic account. If this wasn't you, verify your account security immediately.");

		StatusMessage = "Your password has been set.";

		return RedirectToPage();
	}
}
