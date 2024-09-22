using Immediate.Apis.Shared;
using Immediate.Handlers.Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using Ogma3.Data;
using Ogma3.Infrastructure.ServiceRegistrations;

namespace Ogma3.Areas.Admin.Api.V1.Infractions;

[Handler]
[MapGet("admin/api/infractions/user/{userId:long}")]
[Authorize(Policy = AuthorizationPolicies.RequireAdminOrModeratorRole)]
public static partial class GetUserInfractions
{
	public sealed record Query(long UserId);
	
	private static async ValueTask<Ok<List<Result>>> HandleAsync(
		Query request,
		ApplicationDbContext context,
		CancellationToken cancellationToken
	)
	{
		var infractions = await context.Infractions
			.Where(i => i.UserId == request.UserId)
			.Select(i => new Result(i.Id, i.ActiveUntil, i.RemovedAt != null, i.Reason))
			.ToListAsync(cancellationToken);

		return TypedResults.Ok(infractions);
	}
	
	public sealed record Result(long Id, DateTimeOffset ActiveUntil, bool Removed, string Reason);
}