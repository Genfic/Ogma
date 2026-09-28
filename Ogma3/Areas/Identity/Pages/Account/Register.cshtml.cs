using System.ComponentModel.DataAnnotations;
using System.Text;
using FluentValidation;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.AspNetCore.WebUtilities;
using Ogma3.Data;
using Ogma3.Data.Users;
using Ogma3.Infrastructure.CustomValidators;
using Ogma3.Infrastructure.ServiceRegistrations;
using Ogma3.Services.EmailBlocklistProvider;
using Ogma3.Services.InviteCodeService;
using Ogma3.Services.Mailer;
using Ogma3.Services.PowService;
using Ogma3.Services.SpeedTrapService;
using Ogma3.Services.TurnstileService;
using Ogma3.Services.UserService;
using Routes.Areas.Identity.Pages;

namespace Ogma3.Areas.Identity.Pages.Account;

[AllowAnonymous]
public sealed class RegisterModel(
	UserManager<OgmaUser> userManager,
	SignInManager<OgmaUser> signInManager,
	IMailer emailSender,
	ITurnstileService turnstile,
	PowService powService,
	ISpeedTrapService speedTrap,
	IUserService userService,
	InviteCodeService inviteCodeService,
	ILogger<RegisterModel> logger) : PageModel
{
	[BindProperty] public InputModel Input { get; set; } = new();

	[Required(ErrorMessage = "Turnstile response is required")]
	[BindProperty(Name = "cf-turnstile-response")]
	public string TurnstileResponse { get; set; } = null!;

	public string? ReturnUrl { get; set; }

	public required PowChallenge PowChallenge { get; set; } = null!;

	public IList<AuthenticationScheme> ExternalLogins { get; set; } = null!;

	public sealed class InputModel
	{
		[Display(Name = "Nickname")]
		public string Name { get; init; } = null!;

		[DataType(DataType.EmailAddress)]
		public string Email { get; init; } = null!;

		[DataType(DataType.Password)]
		public string Password { get; init; } = null!;

		[DataType(DataType.Password)]
		[Display(Name = "Confirm password")]
		public string ConfirmPassword { get; init; } = null!;

		[Display(Name = "Invite code")]
		public string? InviteCode { get; set; }

		public bool TosAccepted { get; init; }
		public bool PrivacyPolicyAccepted { get; init; }

		public string? Occupation { get; init; }

		public string SubmissionToken { get; set; } = "";
		public string PowHash { get; init; } = "";
		public string PowToken { get; init; } = "";
		public int PowNonce { get; init; }
	}

	public sealed class InputModelValidation : AbstractValidator<InputModel>
	{
		public InputModelValidation(IEmailBlocklistProvider blocklistProvider, ISpeedTrapService speedTrap)
		{
			RuleFor(im => im.Name)
				.NotEmpty()
				.MinimumLength(CTConfig.User.MinNameLength)
				.MaximumLength(CTConfig.User.MaxNameLength);
			RuleFor(im => im.Email)
				.NotEmpty()
				.EmailAddress()
				.NotDisposable(blocklistProvider);
			RuleFor(im => im.Password)
				.NotEmpty()
				.MinimumLength(CTConfig.User.MinPassLength)
				.MaximumLength(CTConfig.User.MaxPassLength);
			RuleFor(im => im.ConfirmPassword)
				.Equal(im => im.Password);
			RuleFor(im => im.InviteCode)
				.NotEmpty()
				.MinimumLength(10)
				.MaximumLength(18);
			RuleFor(im => im.SubmissionToken)
				.NotEmpty()
				.Must(token => speedTrap.IsHumanSpeed(token, 5))
				.WithMessage("Submission too fast. Slow down and try again.");
			RuleFor(im => im.TosAccepted)
				.Equal(true)
				.WithMessage("You must accept the Terms of Service");
			RuleFor(im => im.PrivacyPolicyAccepted)
				.Equal(true)
				.WithMessage("You must accept the Privacy Policy");
		}
	}

	private async Task Hydrate(string? returnUrl = null, string? invite = null)
	{
		ReturnUrl = returnUrl;
		Input.SubmissionToken = speedTrap.GenerateToken();
		Input.InviteCode ??= invite;
		ExternalLogins = (await signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
		PowChallenge = await powService.IssueChallenge();
	}

	public async Task OnGetAsync(string? returnUrl = null, string? invite = null)
	{
		await Hydrate(returnUrl, invite);
	}

	[EnableRateLimiting(policyName: RateLimiting.Registration)]
	public async Task<IActionResult> OnPostAsync(string? returnUrl = null, string? invite = null)
	{
		returnUrl ??= Url.Content("~/");
		await Hydrate(returnUrl, invite);

		if (!ModelState.IsValid) return Page();

		// Check honeypot
		if (!string.IsNullOrEmpty(Input.Occupation))
		{
			ModelState.AddModelError("Suspicious", "Suspicious activity detected. Try again later.");
			logger.LogInformation("Honeypot field was filled out during registration: {Honeypot}.", Input.Occupation);
			return Page();
		}

		// Check Turnstile
		var turnstileResponse = await turnstile.Verify(TurnstileResponse, Request.HttpContext.Connection.RemoteIpAddress?.ToString() ?? string.Empty);
		if (!turnstileResponse.Success)
		{
			ModelState.TryAddModelError("Turnstile", "Incorrect Turnstile response");
			logger.LogInformation("Register attempt with Turnstile errors: {Errors}", turnstileResponse.ErrorCodes);
			return Page();
		}

		// Check PoW
		var powResponse = await powService.VerifyChallenge(Input.PowToken, Input.PowNonce, Input.PowHash);
		if (powResponse is not PowVerificationResult.Ok)
		{
			ModelState.AddModelError("PoW", "Incorrect PoW response");
			logger.LogInformation("PoW verification for {User} failed during registration: {Result}", Input.Name, powResponse.ToStringFast());
			return Page();
		}

		var reservation = await inviteCodeService.ReserveAsync(Input.InviteCode, HttpContext.RequestAborted);

		if (reservation is not InviteCodeReservationResult.Reserved)
		{
			ModelState.TryAddModelError(
				nameof(Input.InviteCode),
				reservation is InviteCodeReservationResult.AlreadyClaimed
					? "This invite code has been used"
					: "Incorrect invite code"
			);
			return Page();
		}

		// Create user
		var result = await userService.CreateAsync(Input.Name, Input.Email, Input.Password);

		// If everything went fine...
		if (result.Succeeded)
		{
			logger.LogInformation("User {Name} created an account!", Input.Name);

			if (!await inviteCodeService.AttachAsync(Input.InviteCode, result.User.Id, HttpContext.RequestAborted))
			{
				// The reservation vanished mid-flight, so this registration cannot be tied to a
				// code. Undo it rather than leave an account nothing is accountable to.
				var undo = await userManager.DeleteAsync(result.User);
				if (!undo.Succeeded)
				{
					logger.LogError("Failed to delete user {UserId} whose invite reservation was lost; code stays consumed.", result.User.Id);
				}

				ModelState.TryAddModelError(nameof(Input.InviteCode), "Incorrect invite code");
				return Page();
			}

			// Send confirmation code
			var code = await userManager.GenerateEmailConfirmationTokenAsync(result.User);
			code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));

			var callbackUrl = Url.Page(
				"/Account/ConfirmEmail",
				null,
				new { area = "Identity", userName = result.User.UserName, code },
				Request.Scheme);

			await emailSender.SendEmailTemplateAsync(Input.Email, "confirm-email", new()
			{
				["name"] = result.User.UserName,
				["link"] = callbackUrl ?? "",
				["product_name"] = "Genfic",
			});

			if (userManager.Options.SignIn.RequireConfirmedAccount)
			{
				return Account_RegisterConfirmation.Get(Input.Email).Redirect(this);
			}

			return LocalRedirect(returnUrl);
		}

		// Nothing was created, so hand the code back rather than burning it on a failed signup.
		await inviteCodeService.ReleaseAsync(Input.InviteCode, HttpContext.RequestAborted);

		foreach (var error in result.Errors)
		{
			ModelState.AddModelError(string.Empty, error.Description);
		}

		// If we got this far, something failed, redisplay form
		return Page();
	}
}