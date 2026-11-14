using System.ComponentModel.DataAnnotations;
using Ogma3.Data;
using System.Text;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Ogma3.Data.SecurityTransactions;
using Ogma3.Data.Users;
using Ogma3.Services.Security;
using Routes.Areas.Identity.Pages;

namespace Ogma3.Areas.Identity.Pages.Account.Manage;

/// <summary>
/// Sets up (or replaces) an authenticator app. The new secret is *staged* in the pending
/// security transaction and only committed once a code generated from it has been verified —
/// the old authenticator keeps working until then, and cancelling never disables MFA.
/// </summary>
public sealed class EnableAuthenticatorModel
(
	OgmaUserManager userManager,
	IUserStore<OgmaUser> userStore,
	SignInManager<OgmaUser> signInManager,
	SecurityAuthorizationService securityAuth,
	SecurityNotificationService notifications,
	TimeProvider timeProvider,
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

	public bool ReplacingExisting { get; private set; }

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

		ReplacingExisting = user.TwoFactorEnabled;

		// Setting up or replacing an authenticator requires fresh account proof
		if (!await securityAuth.HasVerifiedGrantAsync(user, SecurityOperation.SetupTotp))
		{
			var setup = await securityAuth.GetOrCreateTotpSetupAsync(user);
			return ReauthRequired.RedirectToReauth(setup, Request.Path.ToString(), this);
		}

		var txn = await securityAuth.GetOrCreateTotpSetupAsync(user);
		if (txn.ProposedSecret is null)
		{
			// Defensive: the staged secret must exist for a pending setup transaction
			StatusMessage = "Error: the staged authenticator setup is missing. Please try again.";
			return Account_Manage_TwoFactorAuthentication.Get().Redirect(this);
		}

		// The QR code is generated from the *proposed* secret; the current key is untouched
		SharedKey = FormatKey(txn.ProposedSecret);
		var email = await userManager.GetEmailAsync(user);
		AuthenticatorUri = GenerateQrCodeUri(email!, txn.ProposedSecret);

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
			return await RedirectToSetupAsync(user);
		}

		ReplacingExisting = user.TwoFactorEnabled;

		// Setup requires fresh account proof first...
		if (!await securityAuth.HasVerifiedGrantAsync(user, SecurityOperation.SetupTotp))
		{
			var pending = await securityAuth.GetOrCreateTotpSetupAsync(user);
			return ReauthRequired.RedirectToReauth(pending, Request.Path.ToString(), this);
		}

		var setupTxn = await securityAuth.GetOrCreateTotpSetupAsync(user);
		if (setupTxn.ProposedSecret is null)
		{
			StatusMessage = "Error: the staged authenticator setup is missing. Please try again.";
			return Account_Manage_TwoFactorAuthentication.Get().Redirect(this);
		}

		// ...and a code from the *proposed* secret proves the new authenticator works
		var verificationCode = Input.Code.Replace(" ", string.Empty).Replace("-", string.Empty);

		if (!TotpStaging.Verify(setupTxn.ProposedSecret, verificationCode, timeProvider.GetUtcNow()))
		{
			ModelState.AddModelError("Input.Code", "Verification code is invalid.");
			return await RedirectToSetupAsync(user);
		}

		await using var dbTx = await securityAuth.BeginCredentialMutationAsync(user.Id);

		// Consume the grant inside the same transaction as the commit
		if (!await securityAuth.ConsumeAsync(user, SecurityOperation.SetupTotp))
		{
			await dbTx.RollbackAsync();
			var pending = await securityAuth.GetOrCreateTotpSetupAsync(user);
			return ReauthRequired.RedirectToReauth(pending, Request.Path.ToString(), this);
		}

		// Commit the staged secret: the old key stops working only here
		await SetAuthenticatorKeyAsync(user, setupTxn.ProposedSecret);
		var enableResult = await userManager.SetTwoFactorEnabledAsync(user, true);
		if (!enableResult.Succeeded)
		{
			await dbTx.RollbackAsync();
			throw new InvalidOperationException($"Unexpected error occurred setting 2FA for user with ID '{user.Id}'.");
		}

		// Bump the stamp explicitly — replacing an already-enabled key with 2FA already
		// enabled doesn't do it by itself, and other sessions must be revoked
		await userManager.UpdateSecurityStampAsync(user);

		var userId = await userManager.GetUserIdAsync(user);
		logger.LogInformation("User with ID '{UserId}' has enabled 2FA with an authenticator app", userId);

		await securityAuth.InvalidateForUserAsync(user.Id);
		await dbTx.CommitAsync();

		await signInManager.RefreshSignInAsync(user);

		StatusMessage = "Your authenticator app has been verified.";

		await notifications.SendAsync(user, "Two-factor authentication was changed on your account",
			"An authenticator app was just set up or replaced for your Genfic account. If this wasn't you, verify your account security immediately.");

		if (await userManager.CountRecoveryCodesAsync(user) != 0)
		{
			return Account_Manage_TwoFactorAuthentication.Get().Redirect(this);
		}

		var recoveryCodes = await userManager.GenerateNewTwoFactorRecoveryCodesAsync(user, 10);
		RecoveryCodes = recoveryCodes?.ToArray();

		return Account_Manage_ShowRecoveryCodes.Get().Redirect(this);

		async Task<IActionResult> RedirectToSetupAsync(OgmaUser u)
		{
			var setup = await securityAuth.GetOrCreateTotpSetupAsync(u);
			if (setup.VerifiedAt is not null)
			{
				SharedKey = FormatKey(setup.ProposedSecret!);
				var email = await userManager.GetEmailAsync(u);
				AuthenticatorUri = GenerateQrCodeUri(email!, setup.ProposedSecret!);
				return Page();
			}

			return ReauthRequired.RedirectToReauth(setup, Request.Path.ToString(), this);
		}
	}

	/// <summary>
	/// Writes a specific authenticator key through Identity's authenticator key store,
	/// instead of <see cref="UserManager{OgmaUser}.ResetAuthenticatorKeyAsync"/> which
	/// only generates a random one.
	/// </summary>
	private async Task SetAuthenticatorKeyAsync(OgmaUser user, string key)
	{
		if (userStore is not IUserAuthenticatorKeyStore<OgmaUser> authenticatorStore)
		{
			throw new NotSupportedException("The user store does not support authenticator keys.");
		}

		await authenticatorStore.SetAuthenticatorKeyAsync(user, key, CancellationToken.None);
	}

	// The previous implementation reset the key on GET when none was stored — with staged
	// setup the stored key is only touched at commit time, so no read fallback is needed here

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
