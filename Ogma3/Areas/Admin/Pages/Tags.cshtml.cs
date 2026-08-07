using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Ogma3.Infrastructure.ServiceRegistrations;

namespace Ogma3.Areas.Admin.Pages;

[Authorize(AuthorizationPolicies.RequireAdminOrModeratorRole)]
public sealed class Tags : PageModel
{
	// Handler needed for SafeRouting generator to work
	public IActionResult OnGet() => Page();
}