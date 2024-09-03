using Immediate.Apis.Shared;
using Immediate.Handlers.Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using Ogma3.Data;
using Ogma3.Data.Tags;
using Ogma3.Infrastructure.ServiceRegistrations;

namespace Ogma3.Api.V1.Tags;

[Handler]
[MapGet("api/tags/all")]
[Authorize(Policy = AuthorizationPolicies.RequireAdminOrModeratorRole)]
public static partial class GetAllTags
{
	public sealed record Query;

	private static async ValueTask<Ok<TagDto[]>> HandleAsync(Query _, ApplicationDbContext context, CancellationToken cancellationToken)
	{
		var tags = await context.Tags
			.OrderBy(t => t.Id)
			.ProjectToDto()
			.ToArrayAsync(cancellationToken);

		return TypedResults.Ok(tags);
	}
}