using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Ogma3.Areas.Identity.Pages.Account.Manage;

public sealed class ShowRecoveryCodesModel : PageModel
{
	[TempData] public string[]? RecoveryCodes { get; set; }

	[TempData] public required string StatusMessage { get; set; }

	public IActionResult OnGet()
	{
		if (RecoveryCodes is not { Length: > 0 })
		{
			return Routes.Areas.Identity.Pages.Account_Manage_TwoFactorAuthentication.Get().Redirect(this);
		}

		return Page();
	}
}