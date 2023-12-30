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
using Ogma3.Data.Users;
using reCAPTCHA.AspNetCore;
using Serilog;

namespace Ogma3.Areas.Identity.Pages.Account;

[AllowAnonymous]
public class RegisterConfirmationModel : PageModel
{
	private readonly UserManager<OgmaUser> _userManager;
	private readonly IEmailSender _emailSender;
	private readonly IRecaptchaService _reCaptcha;

	public RegisterConfirmationModel(UserManager<OgmaUser> userManager, IEmailSender emailSender, IRecaptchaService reCaptcha)
	{
		_userManager = userManager;
		_emailSender = emailSender;
		_reCaptcha = reCaptcha;
	}

	[BindProperty] public string Email { get; set; } = null!;


	// public string EmailConfirmationUrl { get; set; }

	public async Task<IActionResult> OnGetAsync(string? email)
	{
		if (email is null)
		{
			return RedirectToPage("/Index");
		}

		var user = await _userManager.FindByEmailAsync(email);
		if (user is null)
		{
			return NotFound($"Unable to load user with email '{email}'.");
		}

		Email = email;

		return Page();
	}


	[Required(ErrorMessage = "ReCaptcha is required")]
	[BindProperty(Name = "g-recaptcha-response")]
	public string ReCaptchaResponse { get; set; } = null!;

	// NOTE: Consider throttling it
	public async Task<IActionResult> OnPostAsync(string? returnUrl = null)
	{
		returnUrl ??= Url.Content("~/");

		if (!ModelState.IsValid) return Page();

		// Check ReCaptcha
		var reResponse = await _reCaptcha.Validate(ReCaptchaResponse);
		if (!reResponse.success)
		{
			ModelState.TryAddModelError("ReCaptcha", "Incorrect ReCaptcha");
			return Page();
		}

		var user = await _userManager.FindByEmailAsync(Email);
		if (user is null)
		{
			return NotFound($"Unable to load user with email '{Email}'.");
		}

		Log.Information("User {Name} requested new confirmation email an account!", user.UserName);

		// Send confirmation code
		var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
		code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));

		var callbackUrl = Url.Page(
			"/Account/ConfirmEmail",
			null,
			new { area = "Identity", userName = user.UserName, code },
			Request.Scheme);

		await _emailSender.SendEmailAsync(Email, "Confirm your email",
			$"Please confirm your account by <a href='{HtmlEncoder.Default.Encode(callbackUrl ?? "")}'>clicking here</a>.\n\nAlternatively, go to <pre>/confirm-email</pre> and enter the code <pre>{code}</pre>.");

		if (_userManager.Options.SignIn.RequireConfirmedAccount)
		{
			return RedirectToPage("RegisterConfirmation", new { email = Email });
		}

		return LocalRedirect(returnUrl);
	}
}