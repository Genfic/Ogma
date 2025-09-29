using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Ogma3.Data.Users;
using Routes.Areas.Identity.Pages;

namespace Ogma3.Areas.Identity.Pages.Account;

[AllowAnonymous]
public sealed class LoginModel(SignInManager<OgmaUser> signInManager, ILogger<LoginModel> logger) : PageModel
{

	[BindProperty] public required InputModel Input { get; set; }

	public required List<AuthenticationScheme> ExternalLogins { get; set; } = [];

	public required string ReturnUrl { get; set; }

	[TempData] public required string ErrorMessage { get; set; }

	public sealed class InputModel
	{
		[Required] public required string Name { get; init; }

		[Required]
		[DataType(DataType.Password)]
		public required string Password { get; init; }

		[Display(Name = "Remember me?")] public required bool RememberMe { get; init; }
	}


	private const string HoneypotSessionKey = "LoginHoneypotKey";
	public string? HoneypotFieldName { get; private set; }

	public async Task OnGetAsync(string? returnUrl = null)
	{
		if (!string.IsNullOrEmpty(ErrorMessage))
		{
			ModelState.AddModelError(string.Empty, ErrorMessage);
		}

		returnUrl ??= Url.Content("~/");

		// Clear the existing external cookie to ensure a clean login process
		await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);

		ExternalLogins = (await signInManager.GetExternalAuthenticationSchemesAsync()).ToList();

		ReturnUrl = returnUrl;

		HoneypotFieldName = Guid.NewGuid().ToString("N")[..16];
		HttpContext.Session.SetString(HoneypotSessionKey, HoneypotFieldName);
	}


	public async Task<IActionResult> OnPostAsync(string? returnUrl = null)
	{
		returnUrl ??= Url.Content("~/");

		// Check honeypot
		HoneypotFieldName = HttpContext.Session.GetString(HoneypotSessionKey);
		if (string.IsNullOrEmpty(HoneypotFieldName))
		{
			ModelState.AddModelError("Expired", "Your session has expired. Try again.");
			logger.LogInformation("Session tampered with, honeypot session key was unset during login.");
			return Page();
		}

		var honeypot = Request.Form[HoneypotFieldName].ToString();
		if (!string.IsNullOrEmpty(honeypot))
		{
			ModelState.AddModelError("Suspicious", "Suspicious activity detected. Try again later.");
			logger.LogInformation("Honeypot field was filled out during login: {Honeypot}.", honeypot);
			return Page();
		}
		HttpContext.Session.Remove(HoneypotSessionKey);

		if (!ModelState.IsValid) return Page();

		// This doesn't count login failures towards account lockout
		// To enable password failures to trigger account lockout, set lockoutOnFailure: true
		var result = await signInManager.PasswordSignInAsync(Input.Name, Input.Password, Input.RememberMe, true);
		if (result.Succeeded)
		{
			logger.LogInformation("User logged in");
			return LocalRedirect(returnUrl);
		}

		if (result.RequiresTwoFactor)
		{
			return Account_LoginWith2fa.Get(Input.RememberMe, returnUrl).Redirect(this);
		}

		if (result.IsLockedOut)
		{
			logger.LogWarning("User account locked out");
			return Account_Lockout.Get().Redirect(this);
		}

		ExternalLogins = (await signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
		ModelState.AddModelError(string.Empty, "Invalid login attempt.");
		return Page();
	}
}