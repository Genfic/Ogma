using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Ogma3.Areas.Identity.Pages.Account.Manage;

public sealed class Passkeys : PageModel
{
	public IActionResult OnGet()
	{
		return Page();
	}
}