using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Ogma3.Data;
using Ogma3.Data.Reports;
using Ogma3.Infrastructure.Extensions;
using Ogma3.Infrastructure.ServiceRegistrations;
using Ogma3.Pages.Shared;

namespace Ogma3.Areas.Admin.Pages;

[Authorize(AuthorizationPolicies.RequireAdminOrModeratorRole)]
public sealed class Reports(AppDbContext context) : PageModel
{
	private const int PerPage = 50;

	public required List<ReportDto> ReportsList { get; set; }
	public required Pagination Pagination { get; set; }

	public async Task OnGetAsync([FromQuery] int page = 1)
	{
		ReportsList = await context.Reports
			.OrderByDescending(r => r.ReportDate)
			.Paginate(page, PerPage)
			.ProjectToDto()
			.ToListAsync();
		var count = await context.Reports.CountAsync();

		Pagination = new()
		{
			CurrentPage = page,
			ItemCount = count,
			PerPage = PerPage,
		};
	}
}