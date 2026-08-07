using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Ogma3.Pages;

public sealed class Settings : PageModel
{
	// Handler needed for SafeRouting generator to work
	public IActionResult OnGet() => Page();
}