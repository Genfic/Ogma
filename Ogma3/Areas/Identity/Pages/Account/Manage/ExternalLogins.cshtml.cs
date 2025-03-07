﻿#nullable disable

using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Ogma3.Data.Users;

namespace Ogma3.Areas.Identity.Pages.Account.Manage;

public sealed class ExternalLoginsModel : PageModel
{
	private readonly UserManager<OgmaUser> _userManager;
	private readonly SignInManager<OgmaUser> _signInManager;

	public ExternalLoginsModel(
		UserManager<OgmaUser> userManager,
		SignInManager<OgmaUser> signInManager)
	{
		_userManager = userManager;
		_signInManager = signInManager;
	}

	public required IList<UserLoginInfo> CurrentLogins { get; set; }

	public required IList<AuthenticationScheme> OtherLogins { get; set; }

	public required bool ShowRemoveButton { get; set; }

	[TempData] public required string StatusMessage { get; set; }

	public async Task<IActionResult> OnGetAsync()
	{
		var user = await _userManager.GetUserAsync(User);
		if (user == null)
		{
			return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
		}

		CurrentLogins = await _userManager.GetLoginsAsync(user);
		OtherLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync())
			.Where(auth => CurrentLogins.All(ul => auth.Name != ul.LoginProvider))
			.ToList();
		ShowRemoveButton = user.PasswordHash != null || CurrentLogins.Count > 1;
		return Page();
	}

	public async Task<IActionResult> OnPostRemoveLoginAsync(string loginProvider, string providerKey)
	{
		var user = await _userManager.GetUserAsync(User);
		if (user is null)
		{
			return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
		}

		var result = await _userManager.RemoveLoginAsync(user, loginProvider, providerKey);
		if (!result.Succeeded)
		{
			StatusMessage = "The external login was not removed.";
			return RedirectToPage();
		}

		await _signInManager.RefreshSignInAsync(user);
		StatusMessage = "The external login was removed.";
		return RedirectToPage();
	}

	public async Task<IActionResult> OnPostLinkLoginAsync(string provider)
	{
		// Clear the existing external cookie to ensure a clean login process
		await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);

		// Request a redirect to the external login provider to link a login for the current user
		var redirectUrl = Url.Page("./ExternalLogins", "LinkLoginCallback");
		var properties = _signInManager.ConfigureExternalAuthenticationProperties(provider, redirectUrl, _userManager.GetUserId(User));
		return new ChallengeResult(provider, properties);
	}

	public async Task<IActionResult> OnGetLinkLoginCallbackAsync()
	{
		var user = await _userManager.GetUserAsync(User);
		if (user is null)
		{
			return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
		}

		var info = await _signInManager.GetExternalLoginInfoAsync(await _userManager.GetUserIdAsync(user));
		if (info is null)
		{
			throw new InvalidOperationException($"Unexpected error occurred loading external login info for user with ID '{user.Id}'.");
		}

		var result = await _userManager.AddLoginAsync(user, info);
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