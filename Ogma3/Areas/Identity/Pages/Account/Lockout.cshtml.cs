using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Ogma3.Areas.Identity.Pages.Account;

[AllowAnonymous]
public sealed class LockoutModel : PageModel
{
	public void OnGet()
	{
	}
}