using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Ogma3.Data.Users;

namespace Ogma3.Areas.Identity.Pages.Account.Manage;

public sealed class ExternalLoginsModel
(
	UserManager<OgmaUser> userManager,
	SignInManager<OgmaUser> signInManager)
	: PageModel
{
	public required List<LoginProvider> AllLogins { get; set; }

	[TempData] public required string StatusMessage { get; set; }

	public record LoginProvider(string DisplayName, string Name, bool Active, string? Key);

	public async Task<IActionResult> OnGetAsync()
	{
		var user = await userManager.GetUserAsync(User);
		if (user == null)
		{
			return NotFound($"Unable to load user with ID '{userManager.GetUserId(User)}'.");
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
			return NotFound($"Unable to load user with ID '{userManager.GetUserId(User)}'.");
		}

		var result = await userManager.RemoveLoginAsync(user, loginProvider, providerKey);
		if (!result.Succeeded)
		{
			StatusMessage = "The external login was not removed.";
			return RedirectToPage();
		}

		await signInManager.RefreshSignInAsync(user);
		StatusMessage = "The external login was removed.";
		return RedirectToPage();
	}

	public async Task<IActionResult> OnPostLinkLoginAsync(string provider)
	{
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
		if (user is null)
		{
			return NotFound($"Unable to load user with ID '{userManager.GetUserId(User)}'.");
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

		// Clear the existing external cookie to ensure a clean login process
		await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);

		StatusMessage = "The external login was added.";
		return RedirectToPage();
	}
}