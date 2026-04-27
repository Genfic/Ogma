using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Ogma3.Data;
using Ogma3.Data.ModeratorActions;
using Ogma3.Infrastructure.ServiceRegistrations;
using Ogma3.Pages.Shared;

namespace Ogma3.Areas.Admin.Pages;

[Authorize(AuthorizationPolicies.RequireStaffRole)]
public sealed class ModLog(ApplicationDbContext context) : PageModel
{
	private const int PerPage = 50;

	public required ICollection<ModeratorAction> Actions { get; set; }
	public required Pagination Pagination { get; set; }

	public async Task OnGet([FromQuery] int page = 1)
	{
		Actions = await context.ModeratorActions
			.OrderByDescending(ma => ma.DateTime)
			.Paginate(page, PerPage)
			.ToListAsync();
		var count = await context.ModeratorActions.CountAsync();

		Pagination = new Pagination
		{
			PerPage = PerPage,
			CurrentPage = page,
			ItemCount = count,
		};
	}
}