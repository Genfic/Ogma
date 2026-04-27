using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Ogma3.Infrastructure.ServiceRegistrations;

namespace Ogma3.Areas.Admin.Pages;


[Authorize(AuthorizationPolicies.RequireStaffRole)]
public sealed class Faq : PageModel
{
	public void OnGet()
	{
	}
}