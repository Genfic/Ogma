using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Ogma3.Data.SecurityTransactions;
using Ogma3.Data.Users;
using Ogma3.Services.Security;

namespace Ogma3.Areas.Identity.Pages.Account.Manage;

public sealed class GenerateRecoveryCodesModel(
	UserManager<OgmaUser> userManager,
	SecurityAuthorizationService securityAuth,
	ILogger<GenerateRecoveryCodesModel> logger)
	: PageModel
{
	[TempData] public string[]? RecoveryCodes { get; set; }

	[TempData] public required string StatusMessage { get; set; }

	public async Task<IActionResult> OnGetAsync()
	{
		var user = await userManager.GetUserAsync(User);
		if (user is null)
		{
			return NotFound($"Unable to load user with ID '{userManager.GetUserId(User)}'.");
		}

		var isTwoFactorEnabled = await userManager.GetTwoFactorEnabledAsync(user);

		if (isTwoFactorEnabled) return Page();

		var userId = await userManager.GetUserIdAsync(user);
		throw new InvalidOperationException(
			$"Cannot generate recovery codes for user with ID '{userId}' because they do not have 2FA enabled.");
	}

	public async Task<IActionResult> OnPostAsync()
	{
		var user = await userManager.GetUserAsync(User);
		if (user is null)
		{
			return NotFound($"Unable to load user with ID '{userManager.GetUserId(User)}'.");
		}

		var isTwoFactorEnabled = await userManager.GetTwoFactorEnabledAsync(user);
		var userId = await userManager.GetUserIdAsync(user);
		if (!isTwoFactorEnabled)
		{
			throw new InvalidOperationException(
				$"Cannot generate recovery codes for user with ID '{userId}' as they do not have 2FA enabled.");
		}

		// Regenerating recovery codes invalidates the existing ones — this requires fresh account proof
		if (!await securityAuth.HasVerifiedGrantAsync(user, SecurityOperation.GenerateRecoveryCodes))
		{
			var txn = await securityAuth.BeginAsync(user, SecurityOperation.GenerateRecoveryCodes);
			return ReauthRequired.RedirectToReauth(txn, Request.Path.ToString(), this);
		}

		if (!await securityAuth.ConsumeAsync(user, SecurityOperation.GenerateRecoveryCodes))
		{
			var txn = await securityAuth.BeginAsync(user, SecurityOperation.GenerateRecoveryCodes);
			return ReauthRequired.RedirectToReauth(txn, Request.Path.ToString(), this);
		}

		var recoveryCodes = await userManager.GenerateNewTwoFactorRecoveryCodesAsync(user, 10);
		RecoveryCodes = recoveryCodes?.ToArray();

		await securityAuth.InvalidateForUserAsync(user.Id);

		logger.LogInformation("User with ID '{UserId}' has generated new 2FA recovery codes", userId);
		StatusMessage = "You have generated new recovery codes.";
		return Routes.Areas.Identity.Pages.Account_Manage_ShowRecoveryCodes.Get().Redirect(this);
	}
}