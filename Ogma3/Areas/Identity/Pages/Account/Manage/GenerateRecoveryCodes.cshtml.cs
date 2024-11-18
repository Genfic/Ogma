using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Ogma3.Data.Users;

namespace Ogma3.Areas.Identity.Pages.Account.Manage;

public sealed class GenerateRecoveryCodesModel : PageModel
{
	private readonly UserManager<OgmaUser> _userManager;
	private readonly ILogger<GenerateRecoveryCodesModel> _logger;

	public GenerateRecoveryCodesModel(
		UserManager<OgmaUser> userManager,
		ILogger<GenerateRecoveryCodesModel> logger)
	{
		_userManager = userManager;
		_logger = logger;
	}

	[TempData] public string[]? RecoveryCodes { get; set; }

	[TempData] public required string StatusMessage { get; set; }

	public async Task<IActionResult> OnGetAsync()
	{
		var user = await _userManager.GetUserAsync(User);
		if (user is null)
		{
			return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
		}

		var isTwoFactorEnabled = await _userManager.GetTwoFactorEnabledAsync(user);

		if (isTwoFactorEnabled) return Page();

		var userId = await _userManager.GetUserIdAsync(user);
		throw new InvalidOperationException(
			$"Cannot generate recovery codes for user with ID '{userId}' because they do not have 2FA enabled.");
	}

	public async Task<IActionResult> OnPostAsync()
	{
		var user = await _userManager.GetUserAsync(User);
		if (user is null)
		{
			return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
		}

		var isTwoFactorEnabled = await _userManager.GetTwoFactorEnabledAsync(user);
		var userId = await _userManager.GetUserIdAsync(user);
		if (!isTwoFactorEnabled)
		{
			throw new InvalidOperationException(
				$"Cannot generate recovery codes for user with ID '{userId}' as they do not have 2FA enabled.");
		}

		var recoveryCodes = await _userManager.GenerateNewTwoFactorRecoveryCodesAsync(user, 10);
		RecoveryCodes = recoveryCodes?.ToArray();

		_logger.LogInformation("User with ID '{UserId}' has generated new 2FA recovery codes", userId);
		StatusMessage = "You have generated new recovery codes.";
		return Routes.Areas.Identity.Pages.Account_Manage_ShowRecoveryCodes.Get().Redirect(this);
	}
}