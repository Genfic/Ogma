using Microsoft.AspNetCore.Identity;
using Ogma3.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Ogma3.Data.SecurityTransactions;
using Ogma3.Data.Users;
using Ogma3.Services.Security;
using Routes.Areas.Identity.Pages;

namespace Ogma3.Areas.Identity.Pages.Account.Manage;

public sealed class Disable2FaModel
(
	OgmaUserManager userManager,
	SecurityAuthorizationService securityAuth,
	ILogger<Disable2FaModel> logger) : PageModel
{
	[TempData] public required string StatusMessage { get; set; }

	public async Task<IActionResult> OnGet()
	{
		var user = await userManager.GetUserAsync(User);
		if (user == null)
		{
			return NotFound($"Unable to load user with ID '{userManager.GetUserId(User)}'.");
		}

		if (!await userManager.GetTwoFactorEnabledAsync(user))
		{
			throw new InvalidOperationException(
				$"Cannot disable 2FA for user with ID '{userManager.GetUserId(User)}' as it's not currently enabled.");
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

		if (!await userManager.GetTwoFactorEnabledAsync(user))
		{
			throw new InvalidOperationException(
				$"Cannot disable 2FA for user with ID '{userManager.GetUserId(User)}' as it's not currently enabled.");
		}

		// Disabling 2FA requires fresh account proof
		if (!await securityAuth.HasVerifiedGrantAsync(user, SecurityOperation.DisableTotp))
		{
			var txn = await securityAuth.BeginAsync(user, SecurityOperation.DisableTotp);
			return ReauthRequired.RedirectToReauth(txn, Request.Path.ToString(), this);
		}

		if (!await securityAuth.ConsumeAsync(user, SecurityOperation.DisableTotp))
		{
			var txn = await securityAuth.BeginAsync(user, SecurityOperation.DisableTotp);
			return ReauthRequired.RedirectToReauth(txn, Request.Path.ToString(), this);
		}

		var disable2FaResult = await userManager.SetTwoFactorEnabledAsync(user, false);
		if (!disable2FaResult.Succeeded)
		{
			throw new InvalidOperationException(
				$"Unexpected error occurred disabling 2FA for user with ID '{userManager.GetUserId(User)}'.");
		}

		logger.LogInformation("User with ID '{UserId}' has disabled 2fa", userManager.GetUserId(User));
		StatusMessage = "2fa has been disabled. You can reenable 2fa when you setup an authenticator app";
		return Account_Manage_TwoFactorAuthentication.Get().Redirect(this);
	}
}