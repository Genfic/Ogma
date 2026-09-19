using Immediate.Apis.Shared;
using Immediate.Handlers.Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using Ogma3.Data;
using Ogma3.Data.Infractions;
using Ogma3.Infrastructure.ServiceRegistrations;
using Ogma3.Services;
using Ogma3.Services.ModeratorActionService;
using Ogma3.Services.UserService;

namespace Ogma3.Areas.Admin.Api.V1.Infractions;

using ReturnType = Results<Ok, UnauthorizedHttpResult, NotFound, InternalServerError<string>>;

[Handler]
[MapDelete("admin/api/infractions/{infractionId:long}")]
[Authorize(AuthorizationPolicies.RequireAdminOrModeratorRole)]
public sealed partial class DeactivateInfraction(AppDbContext context, IUserService userService, BanCache cache, IModeratorActionService moderatorActionService)
{
	public sealed record Command(long InfractionId);

	private async ValueTask<ReturnType> HandleAsync(
		Command request,
		CancellationToken cancellationToken
	)
	{
		if (userService.UserId is not {} uid) return TypedResults.Unauthorized();

		var infraction = await context.Infractions
			.Where(i => i.Id == request.InfractionId)
			.FirstOrDefaultAsync(cancellationToken);

		if (infraction is null) return TypedResults.NotFound();

		infraction.RemovedAt = DateTimeOffset.UtcNow;
		infraction.RemovedById = uid;

		moderatorActionService.LogInfractionLifted(
			infraction.UserId,
			infraction.Id,
			infraction.ActiveUntil,
			infraction.RemovedAt,
			infraction.Type);

		await context.SaveChangesAsync(cancellationToken);

		if (infraction.Type != InfractionType.Ban)
		{
			return TypedResults.Ok();
		}

		var res = await cache.Unban(infraction.UserId);
		return res
			? TypedResults.Ok()
			: TypedResults.InternalServerError("Cound not unban user!");
	}
}
