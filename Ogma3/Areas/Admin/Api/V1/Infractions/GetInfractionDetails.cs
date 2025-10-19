using Immediate.Apis.Shared;
using Immediate.Handlers.Shared;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using Ogma3.Data;
using Ogma3.Data.Infractions;
using Ogma3.Infrastructure.ServiceRegistrations;

namespace Ogma3.Areas.Admin.Api.V1.Infractions;

using ReturnType = Results<Ok<InfractionDto>, NotFound>;

[Handler]
[MapGet("admin/api/infractions/{infractionId:long}")]
[Authorize(AuthorizationPolicies.RequireAdminOrModeratorRole)]
public static partial class GetInfractionDetails
{
	internal static void CustomizeEndpoint(IEndpointConventionBuilder endpoint) => endpoint.WithName(nameof(GetInfractionDetails));

	[UsedImplicitly]
	public sealed record Query(long InfractionId);

	private static async ValueTask<ReturnType> HandleAsync(Query request, ApplicationDbContext context, CancellationToken cancellationToken)
	{
		var infraction = await context.Infractions
			.Where(i => i.Id == request.InfractionId)
			.Select(x => new InfractionDto
			{
				Id = x.Id,
				UserUserName = x.User.UserName,
				UserId = x.UserId,
				IssueDate = x.IssueDate,
				ActiveUntil = x.ActiveUntil,
				RemovedAt = x.RemovedAt,
				Reason = x.Reason,
				Type = x.Type,
				IssuedByUserName = x.IssuedBy.UserName,
				RemovedByUserName = x.RemovedBy == null ? null : x.RemovedBy.UserName,
			})
			.FirstOrDefaultAsync(cancellationToken);

		if (infraction is null) return TypedResults.NotFound();

		return TypedResults.Ok(infraction);
	}

}