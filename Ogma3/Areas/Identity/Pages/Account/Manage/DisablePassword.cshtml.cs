using Ogma3.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Ogma3.Data.SecurityTransactions;
using Ogma3.Data.Users;
using Ogma3.Services.Security;
using Routes.Areas.Identity.Pages;

namespace Ogma3.Areas.Identity.Pages.Account.Manage;

/// <summary>
/// Removes password sign-in from the account. Requires a fresh, user-verified passkey
/// assertion and at least one remaining enrolled passkey at commit time.
/// </summary>
public sealed class DisablePasswordModel
(
	OgmaUserManager userManager,
	SignInManager<OgmaUser> signInManager,
	SecurityAuthorizationService securityAuth,
	PasswordResetTokenGuard resetTokenGuard,
	SecurityNotificationService notifications,
	ILogger<DisablePasswordModel> logger)
	: PageModel
{
	public bool HasPassword { get; private set; }
	public int PasskeyCount { get; private set; }

	[TempData] public required string StatusMessage { get; set; }

	public async Task<IActionResult> OnGetAsync()
	{
		var user = await userManager.GetUserAsync(User);
		if (user is null)
		{
			return NotFound($"Unable to load user with ID '{userManager.GetUserId(User)}'.");
		}

		HasPassword = await userManager.HasPasswordAsync(user);
		if (!HasPassword)
		{
			StatusMessage = "Password sign-in is already disabled.";
			return Account_Manage_ChangePassword.Get().Redirect(this);
		}

		PasskeyCount = (await userManager.GetPasskeysAsync(user)).Count;

		return Page();
	}

	public async Task<IActionResult> OnPostAsync()
	{
		var user = await userManager.GetUserAsync(User);
		if (user is null)
		{
			return NotFound($"Unable to load user with ID '{userManager.GetUserId(User)}'.");
		}

		if (!await userManager.HasPasswordAsync(user))
		{
			StatusMessage = "Password sign-in is already disabled.";
			return Account_Manage_ChangePassword.Get().Redirect(this);
		}

		// Disabling the password always requires a fresh passkey assertion — never password + TOTP
		if (!await securityAuth.HasVerifiedGrantAsync(user, SecurityOperation.RemovePassword))
		{
			var txn = await securityAuth.BeginAsync(user, SecurityOperation.RemovePassword);
			return ReauthRequired.RedirectToReauth(txn, Request.Path.ToString(), this);
		}

		await using var dbTx = await securityAuth.BeginCredentialMutationAsync(user.Id);

		if (!await securityAuth.ConsumeAsync(user, SecurityOperation.RemovePassword))
		{
			await dbTx.RollbackAsync();
			var txn = await securityAuth.BeginAsync(user, SecurityOperation.RemovePassword);
			return ReauthRequired.RedirectToReauth(txn, Request.Path.ToString(), this);
		}

		// Re-read authoritative state inside the lock
		var freshUser = await userManager.GetUserAsync(User);
		if (freshUser is null)
		{
			await dbTx.RollbackAsync();
			return NotFound();
		}

		var passkeys = await userManager.GetPasskeysAsync(freshUser);

		// At least one usable enrolled passkey must remain when committing
		if (passkeys.Count < 1)
		{
			await dbTx.RollbackAsync();
			StatusMessage = "Error: you need at least one enrolled passkey before password sign-in can be removed.";
			return Account_Manage_ChangePassword.Get().Redirect(this);
		}

		var result = await userManager.RemovePasswordAsync(freshUser);
		if (!result.Succeeded)
		{
			await dbTx.RollbackAsync();
			StatusMessage = "Error: password sign-in could not be removed.";
			return RedirectToPage();
		}

		// Clear any outstanding password reset token so it cannot restore the removed password
		await resetTokenGuard.ClearAsync(freshUser.Id);

		await securityAuth.InvalidateForUserAsync(freshUser.Id);
		await dbTx.CommitAsync();

		// RemovePasswordAsync bumps the security stamp, revoking other sessions within the
		// revalidation interval; renew the current session's cookie with the new stamp
		await signInManager.RefreshSignInAsync(freshUser);

		logger.LogInformation("User {UserId} removed password sign-in from their account", freshUser.Id);

		await notifications.SendAsync(freshUser, "Password sign-in was removed from your account",
			"""
			Password sign-in was just removed from your Genfic account.
			Future sign-ins require a passkey, or the documented account recovery process.
			We recommend keeping an independent backup credential or recovery method — two passkeys
			in the same sync provider are not two independent backups.
			If this wasn't you, verify your account security immediately.
			""");

		StatusMessage = "Password sign-in has been removed. You can now sign in with a passkey.";

		return Account_Manage_ChangePassword.Get().Redirect(this);
	}
}
