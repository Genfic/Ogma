using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Ogma3.Data.Users;

namespace Ogma3.Areas.Identity.Pages.Account.Manage;

public class TwoFactorAuthenticationModel : PageModel
{
	// private const string AuthenticatorUriFormat = "otpauth://totp/{0}:{1}?secret={2}&issuer={0}";

	private readonly UserManager<OgmaUser> _userManager;
	private readonly SignInManager<OgmaUser> _signInManager;

	public TwoFactorAuthenticationModel(UserManager<OgmaUser> userManager, SignInManager<OgmaUser> signInManager)
	{
		_userManager = userManager;
		_signInManager = signInManager;
	}

	public bool HasAuthenticator { get; set; }

	public int RecoveryCodesLeft { get; set; }

	[BindProperty] public bool Is2FaEnabled { get; set; }

	public bool IsMachineRemembered { get; set; }

	[TempData] public string? StatusMessage { get; set; }

	public async Task<IActionResult> OnGet()
	{
		var user = await _userManager.GetUserAsync(User);
		if (user == null)
		{
			return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
		}

		HasAuthenticator = await _userManager.GetAuthenticatorKeyAsync(user) != null;
		Is2FaEnabled = await _userManager.GetTwoFactorEnabledAsync(user);
		IsMachineRemembered = await _signInManager.IsTwoFactorClientRememberedAsync(user);
		RecoveryCodesLeft = await _userManager.CountRecoveryCodesAsync(user);

		return Page();
	}

	public async Task<IActionResult> OnPost()
	{
		var user = await _userManager.GetUserAsync(User);
		if (user == null)
		{
			return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
		}

		await _signInManager.ForgetTwoFactorClientAsync();
		StatusMessage =
			"The current browser has been forgotten. When you login again from this browser you will be prompted for your 2fa code.";
		return RedirectToPage();
	}
}