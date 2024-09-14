using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Ogma3.Data.Users;

namespace Ogma3.Areas.Identity.Pages.Account;

[AllowAnonymous]
public sealed class LoginModel(SignInManager<OgmaUser> signInManager, ILogger<LoginModel> logger) : PageModel
{

	[BindProperty] public required InputModel Input { get; set; }

	public required IList<AuthenticationScheme> ExternalLogins { get; set; }

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
	}


	public async Task<IActionResult> OnPostAsync(string? returnUrl = null)
	{
		returnUrl ??= Url.Content("~/");

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
			return RedirectToPage("./LoginWith2fa", new { ReturnUrl = returnUrl, Input.RememberMe });
		}

		if (result.IsLockedOut)
		{
			logger.LogWarning("User account locked out");
			return RedirectToPage("./Lockout");
		}

		ModelState.AddModelError(string.Empty, "Invalid login attempt.");
		return Page();
	}
}