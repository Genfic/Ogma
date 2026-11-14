using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Ogma3.Data.SecurityTransactions;
using Ogma3.Data.Users;
using Ogma3.Services.Security;
using Utils.Extensions;

namespace Ogma3.Areas.Identity.Pages.Account.Manage;

public sealed class ExternalLoginsModel
(
	UserManager<OgmaUser> userManager,
	SignInManager<OgmaUser> signInManager,
	SecurityAuthorizationService securityAuth)
	: PageModel
{
	public required List<LoginProvider> AllLogins { get; set; }

	[TempData] public required string StatusMessage { get; set; }

	public sealed record LoginProvider(string DisplayName, string Name, bool Active, string? Key);

	public async Task<IActionResult> OnGetAsync()
	{
		var user = await userManager.GetUserAsync(User);
		if (user == null)
		{
			return NotFound("Unable to find the user. Refresh the page manually.");
		}

		var currentLogins = await userManager.GetLoginsAsync(user);

		var otherLogins = (await signInManager.GetExternalAuthenticationSchemesAsync())
			.ToList();

		AllLogins = otherLogins.Select(l => new LoginProvider(
			l.DisplayName ?? l.Name,
			l.Name,
			currentLogins.Any(ul => ul.LoginProvider == l.Name),
			currentLogins.FirstOrDefault(ul => ul.LoginProvider == l.Name)?.ProviderKey)
		).ToList();

		return Page();
	}

	public async Task<IActionResult> OnPostRemoveLoginAsync(string loginProvider, string providerKey)
	{
		var user = await userManager.GetUserAsync(User);
		if (user is null)
		{
			return NotFound("Unable to find user.");
		}

		// Removing a login requires fresh account proof, and the grant is bound to the
		// specific provider + key
		var target = $"{loginProvider}:{providerKey}";
		if (!await securityAuth.HasVerifiedGrantAsync(user, SecurityOperation.RemoveExternalLogin, target))
		{
			var txn = await securityAuth.BeginAsync(user, SecurityOperation.RemoveExternalLogin, target);
			return ReauthRequired.RedirectToReauth(txn, Request.Path.ToString(), this);
		}

		await using var dbTx = await securityAuth.BeginCredentialMutationAsync(user.Id);

		if (!await securityAuth.ConsumeAsync(user, SecurityOperation.RemoveExternalLogin, target))
		{
			await dbTx.RollbackAsync();
			var txn = await securityAuth.BeginAsync(user, SecurityOperation.RemoveExternalLogin, target);
			return ReauthRequired.RedirectToReauth(txn, Request.Path.ToString(), this);
		}

		// Re-read authoritative state inside the lock
		if (await userManager.GetUserAsync(User) is not {} freshUser)
		{
			await dbTx.RollbackAsync();
			return NotFound("Unable to find user.");
		}

		var logins = await userManager.GetLoginsAsync(freshUser);
		var passkeys = await userManager.GetPasskeysAsync(freshUser);

		// At least one usable primary login path must remain after the removal
		if (!LoginPathPolicy.HasUsablePrimaryLoginPath(
			    passkeys.Count,
			    hasPassword: freshUser.PasswordHash is not null,
			    externalLoginCount: logins.Count - 1))
		{
			await dbTx.RollbackAsync();
			StatusMessage = "Error: you cannot remove your only usable login method.";
			return RedirectToPage();
		}

		var result = await userManager.RemoveLoginAsync(freshUser, loginProvider, providerKey);
		if (!result.Succeeded)
		{
			await dbTx.RollbackAsync();
			StatusMessage = "The external login was not removed.";
			return RedirectToPage();
		}

		// Bump the stamp so other sessions are revoked, and renew the current session's cookie
		await userManager.UpdateSecurityStampAsync(freshUser);
		await securityAuth.InvalidateForUserAsync(freshUser.Id);
		await dbTx.CommitAsync();
		await signInManager.RefreshSignInAsync(freshUser);

		StatusMessage = "The external login was removed.";
		return RedirectToPage();
	}

	public async Task<IActionResult> OnPostLinkLoginAsync(string provider)
	{
		var user = await userManager.GetUserAsync(User);
		if (user is null)
		{
			return NotFound("Unable to find user.");
		}

		// Linking a new external login requires fresh account proof before the provider challenge;
		// the grant is consumed at the callback, together with the actual change
		if (!await securityAuth.HasVerifiedGrantAsync(user, SecurityOperation.LinkExternalLogin))
		{
			var txn = await securityAuth.BeginAsync(user, SecurityOperation.LinkExternalLogin);
			return ReauthRequired.RedirectToReauth(txn, Request.Path.ToString(), this);
		}

		// Clear the existing external cookie to ensure a clean login process
		await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);

		// Request a redirect to the external login provider to link a login for the current user
		var redirectUrl = Url.Page("./ExternalLogins", "LinkLoginCallback");
		var properties = signInManager.ConfigureExternalAuthenticationProperties(provider, redirectUrl, userManager.GetUserId(User));
		return new ChallengeResult(provider, properties);
	}

	public async Task<IActionResult> OnGetLinkLoginCallbackAsync()
	{
		var user = await userManager.GetUserAsync(User);

		// Fallback, in case the browser or the user forces cookies to be `SameSite=Strict`
		if (user is null)
		{
			var externalInfo = await signInManager.GetExternalLoginInfoAsync();
			var xsrfUserId = externalInfo?.AuthenticationProperties?.Items.GetOrDefault("XsrfId");
			if (xsrfUserId is not null)
			{
				user = await userManager.FindByIdAsync(xsrfUserId);
			}
		}

		if (user is null)
		{
			return NotFound("Unable to find user.");
		}

		// The grant was verified before the provider challenge; consume it with the mutation
		if (!await securityAuth.ConsumeAsync(user, SecurityOperation.LinkExternalLogin))
		{
			StatusMessage = "The external login was not added: account verification is missing or expired. Start the link again.";
			return RedirectToPage();
		}

		var info = await signInManager.GetExternalLoginInfoAsync(await userManager.GetUserIdAsync(user));
		if (info is null)
		{
			throw new InvalidOperationException($"Unexpected error occurred loading external login info for user with ID '{user.Id}'.");
		}

		var result = await userManager.AddLoginAsync(user, info);
		if (!result.Succeeded)
		{
			StatusMessage = "The external login was not added. External logins can only be associated with one account.";
			return RedirectToPage();
		}

		await securityAuth.InvalidateForUserAsync(user.Id);

		// Clear the existing external cookie to ensure a clean login process
		await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);

		StatusMessage = "The external login was added.";
		return RedirectToPage();
	}
}