using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Text.Encodings.Web;
using FluentValidation;
using Flurl;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;
using Ogma3.Data;
using Ogma3.Data.Users;
using Ogma3.Services.TurnstileService;

namespace Ogma3.Areas.Identity.Pages.Account;

[AllowAnonymous]
public sealed class RegisterModel(
	UserManager<OgmaUser> userManager,
	SignInManager<OgmaUser> signInManager,
	IEmailSender emailSender,
	ITurnstileService turnstile,
	ApplicationDbContext context,
	OgmaConfig config,
	ILogger<RegisterModel> logger) : PageModel
{
	[BindProperty] public InputModel Input { get; set; } = null!;

	[Required(ErrorMessage = "Turnstile response is required")]
	[BindProperty(Name = "cf-turnstile-response")]
	public string TurnstileResponse { get; set; } = null!;

	public string? ReturnUrl { get; set; }

	public IList<AuthenticationScheme> ExternalLogins { get; set; } = null!;

	public sealed class InputModel
	{
		public string Name { get; init; } = null!;
		public string Email { get; init; } = null!;

		[DataType(DataType.Password)] public string Password { get; init; } = null!;

		[DataType(DataType.Password)]
		[Display(Name = "Confirm password")]
		public string ConfirmPassword { get; init; } = null!;

		[Display(Name = "Invite code")] public string? InviteCode { get; init; }
	}

	public sealed class InputModelValidation : AbstractValidator<InputModel>
	{
		public InputModelValidation()
		{
			RuleFor(im => im.Name)
				.NotEmpty()
				.MinimumLength(CTConfig.CUser.MinNameLength)
				.MaximumLength(CTConfig.CUser.MaxNameLength);
			RuleFor(im => im.Email)
				.NotEmpty()
				.EmailAddress();
			RuleFor(im => im.Password)
				.NotEmpty()
				.MinimumLength(CTConfig.CUser.MinPassLength)
				.MaximumLength(CTConfig.CUser.MaxPassLength);
			RuleFor(im => im.ConfirmPassword)
				.Equal(im => im.Password);
			RuleFor(im => im.InviteCode)
				.NotEmpty()
				.Length(13);
		}
	}

	public async Task OnGetAsync(string? returnUrl = null)
	{
		ReturnUrl = returnUrl;
		ExternalLogins = (await signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
	}

	public async Task<IActionResult> OnPostAsync(string? returnUrl = null)
	{
		returnUrl ??= Url.Content("~/");
		ExternalLogins = (await signInManager.GetExternalAuthenticationSchemesAsync()).ToList();

		if (!ModelState.IsValid) return Page();
		
		// Check Turnstile
		var turnstileResponse = await turnstile.Verify(TurnstileResponse, Request.HttpContext.Connection.RemoteIpAddress?.ToString() ?? string.Empty);
		if (!turnstileResponse.Success)
		{
			ModelState.TryAddModelError("Turnstile", "Incorrect Turnstile response");
			logger.LogInformation("Register attempt with Turnstile errors: {Errors}", (object)turnstileResponse.ErrorCodes);
			return Page();
		}

		// Check if invite code is correct
		var inviteCode = await context.InviteCodes
			.Where(ic => Input.InviteCode != null && ic.NormalizedCode == Input.InviteCode.ToUpper())
			.FirstOrDefaultAsync();
		if (inviteCode is null)
		{
			ModelState.TryAddModelError("InviteCode", "Incorrect invite code");
			return Page();
		}

		if (inviteCode.UsedDate is not null)
		{
			ModelState.TryAddModelError("InviteCode", "This invite code has been used");
			return Page();
		}

		// Generate Gravatar
		var avatar = new Url(config.AvatarServiceUrl).AppendPathSegment($"{Input.Name}.png").ToString()!;

		// Create user
		var user = new OgmaUser
		{
			UserName = Input.Name,
			Email = Input.Email,
			Avatar = avatar,
		};
		var result = await userManager.CreateAsync(user, Input.Password);

		// If everything went fine...
		if (result.Succeeded)
		{
			logger.LogInformation("User {Name} created an account!", Input.Name);

			// Modify invite code
			inviteCode.UsedBy = user;
			inviteCode.UsedDate = DateTimeOffset.UtcNow;
			await context.SaveChangesAsync();

			// Send confirmation code
			var code = await userManager.GenerateEmailConfirmationTokenAsync(user);
			code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));

			var callbackUrl = Url.Page(
				"/Account/ConfirmEmail",
				null,
				new { area = "Identity", userName = user.UserName, code },
				Request.Scheme);

			await emailSender.SendEmailAsync(Input.Email, "Confirm your email",
				$"Please confirm your account by <a href='{HtmlEncoder.Default.Encode(callbackUrl ?? "")}'>clicking here</a>.\n\nAlternatively, go to <pre>/confirm-email</pre> and enter the code <pre>{code}</pre>.");

			if (userManager.Options.SignIn.RequireConfirmedAccount)
			{
				return Routes.Areas.Identity.Pages.Account_RegisterConfirmation.Get(Input.Email).Redirect(this);
			}

			return LocalRedirect(returnUrl);
		}

		foreach (var error in result.Errors)
		{
			ModelState.AddModelError(string.Empty, error.Description);
		}

		// If we got this far, something failed, redisplay form
		return Page();
	}
}