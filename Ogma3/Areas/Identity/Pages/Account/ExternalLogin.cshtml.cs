using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using System.Text;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using Ogma3.Data;
using Ogma3.Data.Images;
using Ogma3.Data.Users;
using Ogma3.Services.OAuthProviders.Patreon;
using Routes.Areas.Identity.Pages;
using Utils;

namespace Ogma3.Areas.Identity.Pages.Account;

[AllowAnonymous]
public sealed class ExternalLoginModel
(
	SignInManager<OgmaUser> signInManager,
	UserManager<OgmaUser> userManager,
	ILogger<ExternalLoginModel> logger,
	IEmailSender emailSender,
	OgmaConfig config)
	: PageModel
{

	[BindProperty] public required InputModel Input { get; set; }

	public required string LoginProvider { get; set; }

	public required string ReturnUrl { get; set; }

	[TempData] public required string ErrorMessage { get; set; }

	public sealed class InputModel
	{
		[Required] public required string UserName { get; set; }
		[Required][EmailAddress] public required string Email { get; set; }
	}

	public IActionResult OnGet()
	{
		return Account_Login.Get().Redirect(this);
	}

	public IActionResult OnPost(string provider, string? returnUrl = null)
	{
		// Request a redirect to the external login provider.
		var redirectUrl = Url.Page("./ExternalLogin", "Callback", new { returnUrl });
		var properties = signInManager.ConfigureExternalAuthenticationProperties(provider, redirectUrl);
		return new ChallengeResult(provider, properties);
	}

	public async Task<IActionResult> OnGetCallbackAsync(string? returnUrl = null, string? remoteError = null)
	{
		returnUrl ??= Url.Content("~/");
		if (remoteError != null)
		{
			ErrorMessage = $"Error from external provider: {remoteError}";
			return Account_Login.Get(returnUrl).Redirect(this);
		}

		var info = await signInManager.GetExternalLoginInfoAsync();
		if (info is null)
		{
			ErrorMessage = "Error loading external login information.";
			return Account_Login.Get(returnUrl).Redirect(this);
		}

		// Sign in the user with this external login provider if the user already has a login.
		var result = await signInManager.ExternalLoginSignInAsync(info.LoginProvider, info.ProviderKey, false, true);
		if (result.Succeeded)
		{
			logger.LogInformation("{Name} logged in with {LoginProvider} provider", info.Principal.Identity?.Name, info.LoginProvider);
			return LocalRedirect(returnUrl);
		}

		if (result.IsLockedOut)
		{
			return Account_Lockout.Get().Redirect(this);
		}

		// If the user does not have an account, then ask the user to create an account.
		ReturnUrl = returnUrl;
		LoginProvider = info.LoginProvider;

		var vanity = info.Principal.FindFirstValue(PatreonAuthenticationConstants.Claims.Vanity);
		var name = info.Principal.FindFirstValue(ClaimTypes.Name);
		var email = info.Principal.FindFirstValue(ClaimTypes.Email);

		Input = new InputModel
		{
			UserName = vanity ?? name ?? email?.Split('@')[0] ?? "",
			Email = email ?? "",
		};

		return Page();
	}

	public async Task<IActionResult> OnPostConfirmationAsync(string? returnUrl = null)
	{
		returnUrl ??= Url.Content("~/");
		// Get the information about the user from the external login provider
		var info = await signInManager.GetExternalLoginInfoAsync();
		if (info is null)
		{
			ErrorMessage = "Error loading external login information during confirmation.";
			return Account_Login.Get(returnUrl).Redirect(this);
		}

		var avatar = info.Principal.FindFirstValue(PatreonAuthenticationConstants.Claims.Avatar);

		if (ModelState.IsValid)
		{
			var user = new OgmaUser
			{
				UserName = Input.UserName,
				Email = Input.Email,
				Avatar = new Image
				{
					Url = Path.Join(config.Cdn, avatar ?? Gravatar.Generate(Input.Email)),
				},
			};
			var result = await userManager.CreateAsync(user);
			if (result.Succeeded)
			{
				result = await userManager.AddLoginAsync(user, info);
				if (result.Succeeded)
				{
					await signInManager.SignInAsync(user, false);
					logger.LogInformation("User created an account using {Name} provider", info.LoginProvider);

					var userId = await userManager.GetUserIdAsync(user);
					var code = await userManager.GenerateEmailConfirmationTokenAsync(user);
					code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
					var callbackUrl = Url.Page(
						"/Account/ConfirmEmail",
						null,
						new { area = "Identity", userId, code },
						Request.Scheme);

					await emailSender.SendEmailAsync(Input.Email, "Confirm your email",
						$"Please confirm your account by <a href='{HtmlEncoder.Default.Encode(callbackUrl!)}'>clicking here</a>.");

					return LocalRedirect(returnUrl);
				}
			}

			foreach (var error in result.Errors)
			{
				ModelState.AddModelError(string.Empty, error.Description);
			}
		}

		LoginProvider = info.LoginProvider;
		ReturnUrl = returnUrl;
		return Page();
	}
}