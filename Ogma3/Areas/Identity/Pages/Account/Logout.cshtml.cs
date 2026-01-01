using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Ogma3.Data.Users;
using Ogma3.Infrastructure.Attributes;
using Index = Routes.Pages.Index;

namespace Ogma3.Areas.Identity.Pages.Account;

[AllowAnonymous]
[AllowBannedUsers]
public sealed class LogoutModel(SignInManager<OgmaUser> signInManager) : PageModel
{
	public async Task<IActionResult> OnGetAsync(string? returnUrl = null)
	{
		await signInManager.SignOutAsync();

		return returnUrl is null
			? Index.Get().Redirect(this)
			: RedirectToPage(returnUrl);
	}
}