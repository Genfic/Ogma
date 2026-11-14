using System.Text;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using Ogma3.Data.Users;
using Ogma3.Services.Security;

namespace Ogma3.Areas.Identity.Pages.Account;

[AllowAnonymous]
public sealed class ConfirmEmailChangeModel(
	UserManager<OgmaUser> userManager,
	SignInManager<OgmaUser> signInManager,
	SecurityAuthorizationService securityAuth,
	SecurityNotificationService notifications)
	: PageModel
{
	[TempData] public required string StatusMessage { get; set; }

	[BindProperty] public string? UserName { get; set; }
	[BindProperty] public string? Email { get; set; }
	[BindProperty] public string? Code { get; set; }

	public bool Success { get; private set; }
	public bool InvalidToken { get; private set; }

	/// <summary>
	/// Validates the change-email link without mutating anything; the actual change is
	/// applied through the POST form, keeping confirmation GETs non-mutating.
	/// </summary>
	public async Task<IActionResult> OnGetAsync(string userName, string email, string code)
	{
		UserName = userName;
		Email = email;
		Code = code;

		var user = await userManager.FindByNameAsync(userName);
		if (user is null)
		{
			return NotFound($"Unable to load user with name '{userName}'.");
		}

		var decoded = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(code));
		var valid = await userManager.VerifyUserTokenAsync(
			user, userManager.Options.Tokens.ChangeEmailTokenProvider,
			UserManager<OgmaUser>.GetChangeEmailTokenPurpose(email), decoded);

		InvalidToken = !valid;
		return Page();
	}

	public async Task<IActionResult> OnPostAsync()
	{
		if (string.IsNullOrEmpty(UserName) || string.IsNullOrEmpty(Email) || string.IsNullOrEmpty(Code))
		{
			return NotFound();
		}

		var user = await userManager.FindByNameAsync(UserName);
		if (user is null)
		{
			return NotFound($"Unable to load user with name '{UserName}'.");
		}

		var oldEmail = await userManager.GetEmailAsync(user);

		var decoded = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(Code));
		var result = await userManager.ChangeEmailAsync(user, Email, decoded);
		if (!result.Succeeded)
		{
			StatusMessage = "Error changing email.";
			return Page();
		}

		// In our UI email and username are one and the same, so when we update the email,
		// we need to update the username.
		var setUserNameResult = await userManager.SetUserNameAsync(user, Email);
		if (!setUserNameResult.Succeeded)
		{
			StatusMessage = "Error changing user name.";
			return Page();
		}

		// The email change bumps the security stamp; renew the current session's cookie and
		// invalidate outstanding security transactions bound to the previous stamp
		await signInManager.RefreshSignInAsync(user);
		await securityAuth.InvalidateForUserAsync(user.Id);

		Success = true;

		if (oldEmail is not null)
		{
			await notifications.SendAsync(user, "Your account email was changed",
				$"""
				The email address of your Genfic account was changed to {Email}.
				If this wasn't you, contact the site staff immediately.
				""");
		}

		return Page();
	}
}
