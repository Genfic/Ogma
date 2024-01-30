#nullable disable

using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;
using Ogma3.Data.Users;
using Ogma3.Services.TurnstileService;

namespace Ogma3.Areas.Identity.Pages.Account;

[AllowAnonymous]
public class ForgotPasswordModel(
	UserManager<OgmaUser> userManager,
	IEmailSender emailSender,
	ITurnstileService turnstile,
	ILogger<ForgotPasswordModel> logger) : PageModel
{
	[BindProperty] public required InputModel Input { get; set; }
	
	[Required(ErrorMessage = "Turnstile response is required")]
	[BindProperty(Name = "cf-turnstile-response")]
	public string TurnstileResponse { get; set; } = null!;

	public class InputModel
	{
		[Required,
		 EmailAddress]
		public required string Email { get; set; }
	}

	public async Task<IActionResult> OnPostAsync()
	{
		if (!ModelState.IsValid) return Page();

		var user = await userManager.FindByEmailAsync(Input.Email);
		if (user == null || !await userManager.IsEmailConfirmedAsync(user))
		{
			// Don't reveal that the user does not exist or is not confirmed
			return RedirectToPage("./ForgotPasswordConfirmation");
		}
		
		// Check Turnstile
		var turnstileResponse = await turnstile.Verify(TurnstileResponse, Request.HttpContext.Connection.RemoteIpAddress?.ToString() ?? string.Empty);
		if (!turnstileResponse.Success)
		{
			ModelState.TryAddModelError("Turnstile", "Incorrect Turnstile response");
			logger.LogInformation("Forgot password attempt with Turnstile errors: {Errors}", (object)turnstileResponse.ErrorCodes);
			return Page();
		}

		// For more information on how to enable account confirmation and password reset please 
		// visit https://go.microsoft.com/fwlink/?LinkID=532713
		var code = await userManager.GeneratePasswordResetTokenAsync(user);
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

		return RedirectToPage("./ForgotPasswordConfirmation");
	}
}