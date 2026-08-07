using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Ogma3.Areas.Identity.Pages.Account;

public sealed class AccessDeniedModel : PageModel
{
	// Handler needed for SafeRouting generator to work
	public IActionResult OnGet() => Page();
}