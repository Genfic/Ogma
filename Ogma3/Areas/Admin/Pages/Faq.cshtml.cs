using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Ogma3.Areas.Admin.Pages;

[Authorize]
public sealed class Faq : PageModel
{
	public void OnGet()
	{
	}
}