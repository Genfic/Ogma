using Immediate.Apis.Shared;
using Immediate.Handlers.Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using Ogma3.Data;
using Ogma3.Infrastructure.ServiceRegistrations;

namespace Ogma3.Areas.Admin.Api.V1.Telemetry;

[Handler]
[MapGet($"admin/api/telemetry/{nameof(GetImportantItemCounts)}")]
[Authorize(Policy = AuthorizationPolicies.RequireAdminRole)]
public static partial class GetImportantItemCounts
{
	public sealed record Query;

	private static async ValueTask<Ok<Dictionary<string, int>>> HandleAsync(
		Query _,
		ApplicationDbContext context,
		CancellationToken cancellationToken
	)
	{
		const string sql =
			"""
			SELECT 'Stories' as name, count(1) as count FROM "Stories" UNION
			SELECT 'Chapters' as name, count(1) as count FROM "Chapters" UNION
			SELECT 'Blogposts' as name, count(1) as count FROM "Blogposts" UNION
			SELECT 'Users' as name, count(1) as count FROM "AspNetUsers" u WHERE u."EmailConfirmed" UNION
			SELECT 'Comments' as name, count(1) as count FROM "Comments" c WHERE c."DeletedBy" is null UNION
			SELECT 'Reports' as name, count(1) as count FROM "Reports";
			""";
		
		var counts = await context.Database
			.SqlQueryRaw<(string Name, int Count)>(sql)
			.ToDictionaryAsync(i => i.Name, i => i.Count, cancellationToken);

		return TypedResults.Ok(counts);
	}
}