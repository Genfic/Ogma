using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Ogma3.Data.Users;
using Routes.Areas.Identity.Pages;

namespace Ogma3.Areas.Identity.Pages.Account;

[AllowAnonymous]
public sealed class LoginWith2FaModel(SignInManager<OgmaUser> signInManager, ILogger<LoginWith2FaModel> logger)
	: PageModel
{
	[BindProperty] public required InputModel Input { get; set; }

	public required bool RememberMe { get; set; }

	public string? ReturnUrl { get; set; }

	public sealed class InputModel
	{
		[Required]
		[StringLength(7, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
		[DataType(DataType.Text)]
		[Display(Name = "Authenticator code")]
		public required string TwoFactorCode { get; set; }

		[Display(Name = "Remember this machine")]
		public required bool RememberMachine { get; set; }
	}

	public async Task<IActionResult> OnGetAsync(bool rememberMe, string? returnUrl = null)
	{
		// Ensure the user has gone through the username and password screen first
		var user = await signInManager.GetTwoFactorAuthenticationUserAsync();

		if (user is null)
		{
			throw new InvalidOperationException("Unable to load two-factor authentication user.");
		}

		ReturnUrl = returnUrl;
		RememberMe = rememberMe;

		return Page();
	}

	public async Task<IActionResult> OnPostAsync(bool rememberMe, string? returnUrl = null)
	{
		if (!ModelState.IsValid)
		{
			return Page();
		}

		returnUrl ??= Url.Content("~/");

		var user = await signInManager.GetTwoFactorAuthenticationUserAsync();
		if (user == null)
		{
			throw new InvalidOperationException("Unable to load two-factor authentication user.");
		}

		var authenticatorCode = Input.TwoFactorCode.Replace(" ", string.Empty).Replace("-", string.Empty);

		var result = await signInManager.TwoFactorAuthenticatorSignInAsync(authenticatorCode, rememberMe, Input.RememberMachine);

		if (result.Succeeded)
		{
			logger.LogInformation("User with ID '{UserId}' logged in with 2fa", user.Id);
			return LocalRedirect(returnUrl);
		}

		if (result.IsLockedOut)
		{
			logger.LogWarning("User with ID '{UserId}' account locked out", user.Id);
			return Account_Lockout.Get().Redirect(this);
		}

		logger.LogWarning("Invalid authenticator code entered for user with ID '{UserId}'", user.Id);
		ModelState.AddModelError(string.Empty, "Invalid authenticator code.");
		return Page();
	}
}