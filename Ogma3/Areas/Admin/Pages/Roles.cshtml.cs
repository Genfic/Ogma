using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Ogma3.Data.Roles;

namespace Ogma3.Areas.Admin.Pages;

public sealed class Roles : PageModel
{
	[BindProperty] public required OgmaRole Input { get; set; }

	public void OnGet()
	{
	}
}