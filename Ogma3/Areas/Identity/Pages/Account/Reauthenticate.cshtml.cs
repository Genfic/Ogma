using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.RateLimiting;
using Ogma3.Data;
using Ogma3.Data.SecurityTransactions;
using Ogma3.Data.Users;
using Ogma3.Infrastructure.ServiceRegistrations;
using Ogma3.Services.Security;
using Routes.Areas.Identity.Pages;

namespace Ogma3.Areas.Identity.Pages.Account;

[Authorize]
public sealed class ReauthenticateModel(
	SecurityAuthorizationService securityAuth,
	OgmaUserManager userManager,
	SignInManager<OgmaUser> signInManager)
	: PageModel
{
	/// <summary>Generic failure/status message — never reveals which factor failed.</summary>
	public string? Message { get; private set; }

	public SecurityTransaction? Transaction { get; private set; }
	public string? DisplayName { get; private set; }
	public bool HasPassword { get; private set; }
	public bool TotpEnabled { get; private set; }
	public bool HasPasskeys { get; private set; }
	public List<(string Name, string DisplayName)> ExternalProviders { get; private set; } = [];

	/// <summary>Validated local destination after (successful or cancelled) verification.</summary>
	public string ReturnUrl { get; private set; } = "";

	private const string DefaultReturnUrl = "/";

	// ======================================================================
	// GET
	// ======================================================================

	public async Task<IActionResult> OnGetAsync(string? transaction, string? returnUrl = null)
	{
		if (await userManager.GetUserAsync(User) is not { } user)
		{
			// Absent or invalid application session → route through ordinary login
			var loginUrl = Account_Login.Get(returnUrl).Url(Url);
			return Redirect(loginUrl);
		}

		DisplayName = await userManager.GetUserNameAsync(user);
		ReturnUrl = SanitizeReturnUrl(returnUrl);

		if (string.IsNullOrEmpty(transaction))
		{
			Message = "There is no pending security action to verify.";
			return Page();
		}

		var (txn, rejection) = await securityAuth.GetPendingAsync(transaction, user);
		if (txn is null || rejection is not SecurityTransactionRejection.None)
		{
			Message = rejection switch
			{
				SecurityTransactionRejection.Expired => "That security action expired. Please start it again.",
				SecurityTransactionRejection.StampStale => "That security action is no longer valid because your account security details changed. Please start it again.",
				_ => "That security action could not be found. Please start it again.",
			};
			return Page();
		}

		// A transaction that has already been verified goes straight back to the pending operation;
		// the operation itself will consume the grant through a validated POST.
		if (txn.VerifiedAt is not null)
		{
			return LocalRedirect(ReturnUrl);
		}

		Transaction = txn;
		await LoadMethodsAsync(user);
		return Page();
	}

	/// <summary>
	/// Assertion request options for the passkey proof, bound to the current user
	/// and always requiring user verification.
	/// </summary>
	public async Task<IActionResult> OnGetPasskeyOptionsAsync(string transaction)
	{
		if (await userManager.GetUserAsync(User) is not { } user)
		{
			return Unauthorized();
		}

		var (txn, rejection) = await securityAuth.GetPendingAsync(transaction, user);
		if (txn is null || rejection is not SecurityTransactionRejection.None ||
			(txn.AllowedProofs & SecurityProof.Passkey) == 0)
		{
			return Unauthorized();
		}

		var options = await securityAuth.MakePasskeyOptionsAsync(user);
		return Content(options, "application/json");
	}

	// ======================================================================
	// Password + TOTP proof
	// ======================================================================

	[EnableRateLimiting(policyName: RateLimiting.Reauth)]
	public async Task<IActionResult> OnPostVerifyPasswordAsync(
		string transaction, string? returnUrl, string password, string? totpCode)
	{
		if (await userManager.GetUserAsync(User) is not { } user)
		{
			return Account_Login.Get().Redirect(this);
		}

		var result = await securityAuth.VerifyPasswordAsync(transaction, password, totpCode);
		if (!result.Success)
		{
			// Redirect with a generic message — no detail about which factor failed
			TempData[nameof(Message)] = "Verification failed. Please try again.";
			return Account_Reauthenticate.Get(transaction, SanitizeReturnUrl(returnUrl)).Redirect(this);
		}

		TempData[nameof(Message)] = "Verified. You can continue with the security action.";
		return LocalRedirect(SanitizeReturnUrl(returnUrl));
	}

	// ======================================================================
	// Passkey proof
	// ======================================================================

	[EnableRateLimiting(policyName: RateLimiting.Reauth)]
	public async Task<IActionResult> OnPostVerifyPasskeyAsync(
		string transaction, string? returnUrl, string credentials)
	{
		if (await userManager.GetUserAsync(User) is not { } user)
		{
			return Account_Login.Get().Redirect(this);
		}

		var result = await securityAuth.VerifyPasskeyAsync(transaction, credentials);
		if (!result.Success)
		{
			TempData[nameof(Message)] = "Verification failed. Please try again.";
			return Account_Reauthenticate.Get(transaction, SanitizeReturnUrl(returnUrl)).Redirect(this);
		}

		return LocalRedirect(SanitizeReturnUrl(returnUrl));
	}

	// ======================================================================
	// External provider re-verification proof
	// ======================================================================

	public async Task<IActionResult> OnGetChallengeExternalAsync(string transaction, string? returnUrl, string provider)
	{
		// The challenge must come from the authenticated session itself
		if (User.Identity?.IsAuthenticated != true)
		{
			return Account_Login.Get().Redirect(this);
		}

		// Clear the existing external cookie to ensure a clean ceremony
		await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);

		var callbackUrl = Url.Page("./Reauthenticate", "ExternalCallback", new { transaction, returnUrl });
		var properties = signInManager.ConfigureExternalAuthenticationProperties(provider, callbackUrl);
		return new ChallengeResult(provider, properties);
	}

	public async Task<IActionResult> OnGetExternalCallbackAsync(string transaction, string? returnUrl)
	{
		if (await userManager.GetUserAsync(User) is not { } user)
		{
			return Account_Login.Get().Redirect(this);
		}

		var result = await securityAuth.VerifyExternalLoginAsync(transaction);
		if (!result.Success)
		{
			TempData[nameof(Message)] = "Verification failed. The provider identity did not match your account.";
			return Account_Reauthenticate.Get(transaction, SanitizeReturnUrl(returnUrl)).Redirect(this);
		}

		return LocalRedirect(SanitizeReturnUrl(returnUrl));
	}

	// ======================================================================
	// Helpers
	// ======================================================================

	private string SanitizeReturnUrl(string? returnUrl)
		=> !string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl)
			? returnUrl
			: DefaultReturnUrl;

	private async Task LoadMethodsAsync(OgmaUser user)
	{
		HasPassword = user.PasswordHash is not null;
		TotpEnabled = user.TwoFactorEnabled;
		HasPasskeys = (await userManager.GetPasskeysAsync(user)).Count > 0;
		if (!HasPasskeys && !HasPassword)
		{
			var logins = await userManager.GetLoginsAsync(user);
			var schemes = await signInManager.GetExternalAuthenticationSchemesAsync();
			ExternalProviders = logins
				.Select(l => (l.LoginProvider, schemes.FirstOrDefault(s => s.Name == l.LoginProvider)?.DisplayName ?? l.LoginProvider))
				.Distinct()
				.ToList();
		}
	}
}
