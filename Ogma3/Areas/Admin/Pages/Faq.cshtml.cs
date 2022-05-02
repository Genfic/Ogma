using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Ogma3.Areas.Admin.Pages;

[Authorize]
public class Faq : PageModel
{
	public void OnGet()
	{
	}
}