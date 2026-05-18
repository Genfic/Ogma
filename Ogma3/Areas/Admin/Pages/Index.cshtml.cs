using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Ogma3.Data;
using Ogma3.Infrastructure.ServiceRegistrations;

namespace Ogma3.Areas.Admin.Pages;

[Authorize(AuthorizationPolicies.RequireStaffRole)]
public sealed class Index(ApplicationDbContext context) : PageModel
{
	public required List<CountItem> Counts { get; set; }

	public required List<string> LatestActions { get; set; }

	public required Dictionary<string, List<int>> DailyCounts { get; set; }

	public async Task OnGet()
	{
		Counts = await context.Database
			.SqlQueryRaw<CountItem>(EmbeddedResourceQueries.ItemCounts_sql.LoadSql())
			.ToListAsync();

		DailyCounts = await context.Database
			.SqlQueryRaw<DailyCountResult>(EmbeddedResourceQueries.DailyItemCount_sql.LoadSql())
			.ToDictionaryAsync(x => x.Name, x => x.Data.ToList());

		LatestActions = await context.ModeratorActions
			.OrderByDescending(a => a.DateTime)
			.Select(a => a.Description)
			.Take(10)
			.ToListAsync();
	}
}

public sealed record CountItem(int Count, string Name);

public sealed record DailyCountResult(string Name, int[] Data);