using Immediate.Apis.Shared;
using Immediate.Handlers.Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using Ogma3.Data;
using Ogma3.Infrastructure.ServiceRegistrations;

namespace Ogma3.Areas.Admin.Api.V1.Telemetry;

[Handler]
[MapGet($"admin/api/telemetry/{nameof(GetTableInfo)}")]
[Authorize(Policy = AuthorizationPolicies.RequireAdminRole)]
public static partial class GetTableInfo
{
	public sealed record Query;

	private static async ValueTask<Ok<Dictionary<string, ulong>>> HandleAsync(
		Query _,
		ApplicationDbContext context,
		CancellationToken cancellationToken
	)
	{
		const string sql =
			"""
			SELECT
			    table_name as name,
			    pg_relation_size(quote_ident(table_name)) as size
			FROM information_schema.tables
			WHERE table_schema = 'public' AND table_name NOT LIKE '\_\_%'
			ORDER BY size DESC
			""";
		var info = await context.Database
			.SqlQueryRaw<(string Name, ulong Size)>(sql)
			.ToDictionaryAsync(t => t.Name, t => t.Size, cancellationToken);

		return TypedResults.Ok(info);
	}

}