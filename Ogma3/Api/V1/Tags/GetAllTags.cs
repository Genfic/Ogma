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
[Authorize(AuthorizationPolicies.RequireAdminOrModeratorRole)]
public sealed partial class GetAllTags(ApplicationDbContext context)
{

	private async ValueTask<Ok<TagDto[]>> HandleAsync(Query _, CancellationToken cancellationToken)
	{
		var tags = await context.Tags
			.OrderByDescending(t => t.Id)
			.ProjectToDto()
			.ToArrayAsync(cancellationToken);

		return TypedResults.Ok(tags);
	}

	public sealed record Query;
}