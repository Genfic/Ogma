using Microsoft.AspNetCore.Identity;
using Ogma3.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Ogma3.Data.SecurityTransactions;
using Ogma3.Data.Users;
using Ogma3.Services.Security;
using Routes.Areas.Identity.Pages;

namespace Ogma3.Areas.Identity.Pages.Account.Manage;

/// <summary>
/// Destructive reset of the authenticator key: disables 2FA and clears the key.
/// Requires fresh account proof. Prefer the staged replacement on the
/// EnableAuthenticator page, which keeps the old authenticator working until commit.
/// </summary>
public sealed class ResetAuthenticatorModel
(
	OgmaUserManager userManager,
	SignInManager<OgmaUser> signInManager,
	SecurityAuthorizationService securityAuth,
	ILogger<ResetAuthenticatorModel> logger)
	: PageModel
{

	[TempData] public required string StatusMessage { get; set; }

	public async Task<IActionResult> OnGet()
	{
		var user = await userManager.GetUserAsync(User);
		if (user is null)
		{
			return NotFound($"Unable to load user with ID '{userManager.GetUserId(User)}'.");
		}

		return Page();
	}

	public async Task<IActionResult> OnPostAsync()
	{
		var user = await userManager.GetUserAsync(User);
		if (user is null)
		{
			return NotFound($"Unable to load user with ID '{userManager.GetUserId(User)}'.");
		}

		// Resetting the authenticator requires fresh account proof
		if (!await securityAuth.HasVerifiedGrantAsync(user, SecurityOperation.ResetTotp))
		{
			var txn = await securityAuth.BeginAsync(user, SecurityOperation.ResetTotp);
			return ReauthRequired.RedirectToReauth(txn, Request.Path.ToString(), this);
		}

		if (!await securityAuth.ConsumeAsync(user, SecurityOperation.ResetTotp))
		{
			var txn = await securityAuth.BeginAsync(user, SecurityOperation.ResetTotp);
			return ReauthRequired.RedirectToReauth(txn, Request.Path.ToString(), this);
		}

		await userManager.SetTwoFactorEnabledAsync(user, false);
		await userManager.ResetAuthenticatorKeyAsync(user);
		logger.LogInformation("User with ID '{UserId}' has reset their authentication app key", user.Id);

		await securityAuth.InvalidateForUserAsync(user.Id);

		// Both operations bump the security stamp; renew the current session's cookie
		await signInManager.RefreshSignInAsync(user);
		StatusMessage = "Your authenticator app key has been reset, you will need to configure your authenticator app using the new key.";

		return Account_Manage_EnableAuthenticator.Get().Redirect(this);
	}
}
