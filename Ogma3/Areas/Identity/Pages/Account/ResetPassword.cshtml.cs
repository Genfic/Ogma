using System.ComponentModel.DataAnnotations;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using Ogma3.Data.Users;
using Ogma3.Services.Security;
using Ogma3.Services.TurnstileService;
using Routes.Areas.Identity.Pages;

namespace Ogma3.Areas.Identity.Pages.Account;

[AllowAnonymous]
public sealed class ResetPasswordModel(
	UserManager<OgmaUser> userManager,
	ITurnstileService turnstile,
	PasswordResetTokenGuard resetTokenGuard,
	SecurityNotificationService notifications,
	ILogger<ResetPasswordModel> logger)
	: PageModel
{
	[BindProperty] public InputModel Input { get; set; } = null!;

	[Required(ErrorMessage = "Turnstile response is required")]
	[BindProperty(Name = "cf-turnstile-response")]
	public string TurnstileResponse { get; set; } = null!;

	public sealed class InputModel
	{
		[Required,
		 EmailAddress]
		public string Email { get; set; } = "";

		[Required,
		 StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6),
		 DataType(DataType.Password)]
		public string Password { get; set; } = "";

		[DataType(DataType.Password),
		 Display(Name = "Confirm password"),
		 Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
		public string ConfirmPassword { get; set; } = "";

		public string Code { get; set; } = "";
	}

	public IActionResult OnGet(string? code = null)
	{
		if (code is null)
		{
			return BadRequest("A code must be supplied for password reset.");
		}

		Input = new InputModel
		{
			Code = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(code)),
		};
		return Page();
	}

	public async Task<IActionResult> OnPostAsync()
	{
		if (!ModelState.IsValid)
		{
			return Page();
		}

		// Check Turnstile
		var turnstileResponse =
			await turnstile.Verify(TurnstileResponse, Request.HttpContext.Connection.RemoteIpAddress?.ToString() ?? string.Empty);
		if (!turnstileResponse.Success)
		{
			ModelState.TryAddModelError("Turnstile", "Incorrect Turnstile response");
			logger.LogInformation("Password reset attempt with Turnstile errors: {@Errors}", turnstileResponse.ErrorCodes);
			return Page();
		}

		var user = await userManager.FindByEmailAsync(Input.Email);
		if (user is null)
		{
			// Don't reveal that the user does not exist
			return Account_ForgotPasswordConfirmation.Get().Redirect(this);
		}

		logger.LogInformation("Password reset consumption attempt for user {UserId}", user.Id);

		// A passwordless account never regains password authentication through reset endpoints
		if (user.PasswordHash is null)
		{
			logger.LogWarning("Password reset rejected: account {UserId} has no password configured", user.Id);
			return Account_ForgotPasswordConfirmation.Get().Redirect(this);
		}

		// Tokens issued before a password removal are rejected (the marker was cleared), and
		// only the newest outstanding token is consumable
		if (!await resetTokenGuard.ConsumeAsync(user.Id, Input.Code))
		{
			logger.LogWarning("Password reset rejected for user {UserId}: no outstanding reset token matched", user.Id);
			return Account_ForgotPasswordConfirmation.Get().Redirect(this);
		}

		var result = await userManager.ResetPasswordAsync(user, Input.Code, Input.Password);
		if (result.Succeeded)
		{
			logger.LogInformation("Password reset succeeded for user {UserId}", user.Id);

			// ResetPasswordAsync does not touch other credential state (TOTP, passkeys, safety PIN
			// or cooldowns are preserved); notify the account owner
			await notifications.SendAsync(user, "Your password was reset",
				"Your Genfic account password was just reset through the forgot-password flow. If this wasn't you, contact the site staff immediately.");

			return Account_ForgotPasswordConfirmation.Get().Redirect(this);
		}

		foreach (var error in result.Errors)
		{
			ModelState.AddModelError(string.Empty, error.Description);
		}

		return Page();
	}
}