using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Ogma3.Infrastructure.Constants;

namespace Ogma3.Areas.Admin.Pages;

[Authorize(Roles = RoleNames.Admin)]
public class Quotes : PageModel
{
	public void OnGet()
	{
	}
}