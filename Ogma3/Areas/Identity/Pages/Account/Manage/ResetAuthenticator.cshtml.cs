#nullable disable

using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Ogma3.Data.Users;

namespace Ogma3.Areas.Identity.Pages.Account.Manage;

public sealed class ResetAuthenticatorModel : PageModel
{
	private readonly UserManager<OgmaUser> _userManager;
	private readonly SignInManager<OgmaUser> _signInManager;
	private readonly ILogger<ResetAuthenticatorModel> _logger;

	public ResetAuthenticatorModel(
		UserManager<OgmaUser> userManager,
		SignInManager<OgmaUser> signInManager,
		ILogger<ResetAuthenticatorModel> logger)
	{
		_userManager = userManager;
		_signInManager = signInManager;
		_logger = logger;
	}

	[TempData] public required string StatusMessage { get; set; }

	public async Task<IActionResult> OnGet()
	{
		var user = await _userManager.GetUserAsync(User);
		if (user is null)
		{
			return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
		}

		return Page();
	}

	public async Task<IActionResult> OnPostAsync()
	{
		var user = await _userManager.GetUserAsync(User);
		if (user is null)
		{
			return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
		}

		await _userManager.SetTwoFactorEnabledAsync(user, false);
		await _userManager.ResetAuthenticatorKeyAsync(user);
		_logger.LogInformation("User with ID '{UserId}' has reset their authentication app key", user.Id);

		await _signInManager.RefreshSignInAsync(user);
		StatusMessage = "Your authenticator app key has been reset, you will need to configure your authenticator app using the new key.";

		return RedirectToPage("./EnableAuthenticator");
	}
}