using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Ogma3.Data.Users;
using Routes.Areas.Identity.Pages;

namespace Ogma3.Areas.Identity.Pages.Account.Manage;

public sealed class ResetAuthenticatorModel
(
	UserManager<OgmaUser> userManager,
	SignInManager<OgmaUser> signInManager,
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

		await userManager.SetTwoFactorEnabledAsync(user, false);
		await userManager.ResetAuthenticatorKeyAsync(user);
		logger.LogInformation("User with ID '{UserId}' has reset their authentication app key", user.Id);

		await signInManager.RefreshSignInAsync(user);
		StatusMessage = "Your authenticator app key has been reset, you will need to configure your authenticator app using the new key.";

		return Account_Manage_EnableAuthenticator.Get().Redirect(this);
	}
}