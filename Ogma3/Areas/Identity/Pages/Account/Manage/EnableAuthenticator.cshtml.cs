using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Ogma3.Data.Users;

namespace Ogma3.Areas.Identity.Pages.Account.Manage;

public sealed class EnableAuthenticatorModel
(
	UserManager<OgmaUser> userManager,
	ILogger<EnableAuthenticatorModel> logger,
	UrlEncoder urlEncoder)
	: PageModel
{

	private const string AuthenticatorUriFormat = "otpauth://totp/{0}:{1}?secret={2}&issuer={0}&digits=6";

	public required string SharedKey { get; set; }

	public required string AuthenticatorUri { get; set; }

	[TempData] public string[]? RecoveryCodes { get; set; }

	[TempData] public required string StatusMessage { get; set; }

	[BindProperty] public required InputModel Input { get; set; }

	public sealed class InputModel
	{
		[Required]
		[StringLength(7, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
		[DataType(DataType.Text)]
		[Display(Name = "Verification Code")]
		public required string Code { get; init; }
	}

	public async Task<IActionResult> OnGetAsync()
	{
		var user = await userManager.GetUserAsync(User);
		if (user is null)
		{
			return NotFound($"Unable to load user with ID '{userManager.GetUserId(User)}'.");
		}

		await LoadSharedKeyAndQrCodeUriAsync(user);

		return Page();
	}

	public async Task<IActionResult> OnPostAsync()
	{
		var user = await userManager.GetUserAsync(User);
		if (user is null)
		{
			return NotFound($"Unable to load user with ID '{userManager.GetUserId(User)}'.");
		}

		if (!ModelState.IsValid)
		{
			await LoadSharedKeyAndQrCodeUriAsync(user);
			return Page();
		}

		// Strip spaces and hyphens
		var verificationCode = Input.Code.Replace(" ", string.Empty).Replace("-", string.Empty);

		var is2FaTokenValid = await userManager.VerifyTwoFactorTokenAsync(
			user, userManager.Options.Tokens.AuthenticatorTokenProvider, verificationCode);

		if (!is2FaTokenValid)
		{
			ModelState.AddModelError("Input.Code", "Verification code is invalid.");
			await LoadSharedKeyAndQrCodeUriAsync(user);
			return Page();
		}

		await userManager.SetTwoFactorEnabledAsync(user, true);
		var userId = await userManager.GetUserIdAsync(user);
		logger.LogInformation("User with ID '{UserId}' has enabled 2FA with an authenticator app", userId);

		StatusMessage = "Your authenticator app has been verified.";

		if (await userManager.CountRecoveryCodesAsync(user) != 0)
		{
			return Routes.Areas.Identity.Pages.Account_Manage_TwoFactorAuthentication.Get().Redirect(this);
		}

		var recoveryCodes = await userManager.GenerateNewTwoFactorRecoveryCodesAsync(user, 10);
		RecoveryCodes = recoveryCodes?.ToArray();

		return Routes.Areas.Identity.Pages.Account_Manage_ShowRecoveryCodes.Get().Redirect(this);
	}

	private async Task LoadSharedKeyAndQrCodeUriAsync(OgmaUser user)
	{
		// Load the authenticator key & QR code URI to display on the form
		var unformattedKey = await userManager.GetAuthenticatorKeyAsync(user);
		if (string.IsNullOrEmpty(unformattedKey))
		{
			await userManager.ResetAuthenticatorKeyAsync(user);
			unformattedKey = await userManager.GetAuthenticatorKeyAsync(user);
		}

		SharedKey = FormatKey(unformattedKey!);

		var email = await userManager.GetEmailAsync(user);
		AuthenticatorUri = GenerateQrCodeUri(email!, unformattedKey!);
	}

	private static string FormatKey(string unformattedKey)
	{
		var result = new StringBuilder();
		var currentPosition = 0;
		while (currentPosition + 4 < unformattedKey.Length)
		{
			result.Append(unformattedKey.AsSpan(currentPosition, 4)).Append(' ');
			currentPosition += 4;
		}

		if (currentPosition < unformattedKey.Length)
		{
			result.Append(unformattedKey[currentPosition..]);
		}

		return result.ToString().ToLowerInvariant();
	}

	private string GenerateQrCodeUri(string email, string unformattedKey)
	{
		return string.Format(
			AuthenticatorUriFormat,
			urlEncoder.Encode("Genfic"),
			urlEncoder.Encode(email),
			unformattedKey);
	}
}