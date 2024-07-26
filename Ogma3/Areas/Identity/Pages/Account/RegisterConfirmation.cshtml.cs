using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using Ogma3.Data.Users;
using Ogma3.Services.TurnstileService;

namespace Ogma3.Areas.Identity.Pages.Account;

[AllowAnonymous]
public class RegisterConfirmationModel(
	UserManager<OgmaUser> userManager, 
	IEmailSender emailSender, 
	ITurnstileService turnstile,
	ILogger<RegisterConfirmationModel> logger)
	: PageModel
{
	[BindProperty] public string Email { get; set; } = null!;


	// public string EmailConfirmationUrl { get; set; }

	public async Task<IActionResult> OnGetAsync(string? email)
	{
		if (email is null)
		{
			return RedirectToPage("/Index");
		}

		var user = await userManager.FindByEmailAsync(email);
		if (user is null)
		{
			return NotFound($"Unable to load user with email '{email}'.");
		}

		Email = email;

		return Page();
	}
	
	[Required(ErrorMessage = "Turnstile response is required")]
	[BindProperty(Name = "cf-turnstile-response")]
	public string TurnstileResponse { get; set; } = null!;

	// NOTE: Consider throttling it
	public async Task<IActionResult> OnPostAsync(string? returnUrl = null)
	{
		returnUrl ??= Url.Content("~/");

		if (!ModelState.IsValid) return Page();
		
		// Check Turnstile
		var turnstileResponse = await turnstile.Verify(TurnstileResponse, Request.HttpContext.Connection.RemoteIpAddress?.ToString() ?? string.Empty);
		if (!turnstileResponse.Success)
		{
			ModelState.TryAddModelError("Turnstile", "Incorrect Turnstile response");
			logger.LogInformation("Register confirmation attempt with Turnstile errors: {Errors}", (object)turnstileResponse.ErrorCodes);
			return Page();
		}

		var user = await userManager.FindByEmailAsync(Email);
		if (user is null)
		{
			return NotFound($"Unable to load user with email '{Email}'.");
		}

		logger.LogInformation("User {Name} requested new confirmation email an account!", user.UserName);

		// Send confirmation code
		var code = await userManager.GenerateEmailConfirmationTokenAsync(user);
		code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));

		var callbackUrl = Url.Page(
			"/Account/ConfirmEmail",
			null,
			new { area = "Identity", userName = user.UserName, code },
			Request.Scheme);

		await emailSender.SendEmailAsync(Email, "Confirm your email",
			$"Please confirm your account by <a href='{HtmlEncoder.Default.Encode(callbackUrl ?? "")}'>clicking here</a>.\n\nAlternatively, go to <pre>/confirm-email</pre> and enter the code <pre>{code}</pre>.");

		if (userManager.Options.SignIn.RequireConfirmedAccount)
		{
			return RedirectToPage("RegisterConfirmation", new { email = Email });
		}

		return LocalRedirect(returnUrl);
	}
}