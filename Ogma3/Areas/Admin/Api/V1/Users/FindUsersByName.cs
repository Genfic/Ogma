using Immediate.Apis.Shared;
using Immediate.Handlers.Shared;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Ogma3.Data;
using Ogma3.Infrastructure.ServiceRegistrations;

namespace Ogma3.Areas.Admin.Api.V1.Users;

[Handler]
[MapGet("admin/api/users/search/{name}")]
[Authorize(AuthorizationPolicies.RequireAdminOrModeratorRole)]
[UsedImplicitly]
public sealed partial class FindUsersByName(AppDbContext context, ILookupNormalizer normalizer)
{
	[UsedImplicitly]
	public sealed record Query(string Name);

	private async ValueTask<Ok<List<UserSearchResult>>> HandleAsync(Query query, CancellationToken ct)
	{
		var name = normalizer.NormalizeName(query.Name);

		var users = await context.Database
			.SqlQuery<UserSearchResult>( // lang=sql
				$"""
				 SELECT * FROM (
				 	SELECT
				 		u."Id",
				 		u."UserName" as "Name",
				 		u."NormalizedUserName" <-> {name} AS "Distance"
				 	FROM "AspNetUsers" u
				 )
				 WHERE "Distance" < 1.0
				 ORDER BY "Distance"
				 LIMIT 5;
				 """)
			.ToListAsync(ct);

		return TypedResults.Ok(users);
	}

	[UsedImplicitly]
	public sealed record UserSearchResult(long Id, string Name, double Distance);
}