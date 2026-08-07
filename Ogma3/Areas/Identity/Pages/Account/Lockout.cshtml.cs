using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Ogma3.Areas.Identity.Pages.Account;

[AllowAnonymous]
public sealed class LockoutModel : PageModel
{
	// Handler needed for SafeRouting generator to work
	public IActionResult OnGet() => Page();
}