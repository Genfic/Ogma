using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using Ogma3.Data.Users;
using Ogma3.Services.Mailer;
using Ogma3.Services.Security;
using Ogma3.Services.TurnstileService;
using Routes.Areas.Identity.Pages;

namespace Ogma3.Areas.Identity.Pages.Account;

[AllowAnonymous]
public sealed class ForgotPasswordModel(
	UserManager<OgmaUser> userManager,
	IMailer emailSender,
	ITurnstileService turnstile,
	PasswordResetTokenGuard resetTokenGuard,
	ILogger<ForgotPasswordModel> logger) : PageModel
{
	[BindProperty] public required InputModel Input { get; set; }

	[Required(ErrorMessage = "Turnstile response is required")]
	[BindProperty(Name = "cf-turnstile-response")]
	public string TurnstileResponse { get; set; } = null!;

	public sealed class InputModel
	{
		[Required,
		 EmailAddress]
		public required string Email { get; set; }
	}

	public async Task<IActionResult> OnPostAsync()
	{
		if (!ModelState.IsValid) return Page();

		// Check Turnstile
		var turnstileResponse = await turnstile.Verify(TurnstileResponse, Request.HttpContext.Connection.RemoteIpAddress?.ToString() ?? string.Empty);
		if (!turnstileResponse.Success)
		{
			ModelState.TryAddModelError("Turnstile", "Incorrect Turnstile response");
			logger.LogInformation("Forgot password attempt with Turnstile errors: {Errors}", turnstileResponse.ErrorCodes);
			return Page();
		}

		var user = await userManager.FindByEmailAsync(Input.Email);
		if (user == null || !await userManager.IsEmailConfirmedAsync(user))
		{
			// Don't reveal that the user does not exist or is not confirmed
			return Account_ForgotPasswordConfirmation.Get().Redirect(this);
		}

		// An account with no password must not regain password authentication through the
		// ordinary forgot-password flow — audit and respond generically
		if (user.PasswordHash is null)
		{
			logger.LogWarning(
				"Password reset requested for passwordless account (user {UserId}); no reset token was issued", user.Id);

			await emailSender.SendEmailAsync(
				Input.Email,
				"Password reset requested",
				"""
				Someone requested a password reset for your Genfic account.
				Password sign-in is not configured on this account, so no password could be reset.
				If you did not request this, you can safely ignore this email.
				""");

			return Account_ForgotPasswordConfirmation.Get().Redirect(this);
		}

		// For more information on how to enable account confirmation and password reset, please
		// visit https://go.microsoft.com/fwlink/?LinkID=532713
		var code = await userManager.GeneratePasswordResetTokenAsync(user);

		// Single-use marker: only the newest issued token is consumable, and removing the
		// password clears the hash so tokens issued before the removal are rejected
		await resetTokenGuard.IssueAsync(user, code);

		code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
		var callbackUrl = Url.Page(
			"/Account/ResetPassword",
			null,
			new { area = "Identity", code },
			Request.Scheme);

		await emailSender.SendEmailAsync(
			Input.Email,
			"Reset Password",
			$"Please reset your password by <a href='{HtmlEncoder.Default.Encode(callbackUrl!)}'>clicking here</a>.");

		logger.LogInformation("Password reset token issued for user {UserId}", user.Id);

		return Account_ForgotPasswordConfirmation.Get().Redirect(this);
	}
}