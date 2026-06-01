using Immediate.Apis.Shared;
using Immediate.Handlers.Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using Ogma3.Data;
using Ogma3.Data.Infractions;
using Ogma3.Data.ModeratorActions;
using Ogma3.Infrastructure.Constants;
using Ogma3.Infrastructure.Extensions;
using Ogma3.Infrastructure.Middleware;
using Ogma3.Infrastructure.ServiceRegistrations;
using Ogma3.Services.UserService;
using ZiggyCreatures.Caching.Fusion;

namespace Ogma3.Areas.Admin.Api.V1.Infractions;

using ReturnType = Results<Ok, UnauthorizedHttpResult, NotFound>;

[Handler]
[MapDelete("admin/api/infractions/{infractionId:long}")]
[Authorize(AuthorizationPolicies.RequireAdminOrModeratorRole)]
public sealed partial class DeactivateInfraction(ApplicationDbContext context, IUserService userService, IFusionCache cache)
{
	public sealed record Command(long InfractionId);

	private async ValueTask<ReturnType> HandleAsync(
		Command request,
		CancellationToken cancellationToken
	)
	{
		if (userService.UserId is not {} uid) return TypedResults.Unauthorized();
		if (userService.User?.GetUsername() is not {} modName) return TypedResults.Unauthorized();

		var infraction = await context.Infractions
			.Where(i => i.Id == request.InfractionId)
			.FirstOrDefaultAsync(cancellationToken);

		if (infraction is null) return TypedResults.NotFound();

		infraction.RemovedAt = DateTimeOffset.UtcNow;
		infraction.RemovedById = uid;

		var action = new ModeratorAction
		{
			StaffMemberId = uid,
			Description = ModeratorActionTemplates.Infractions.Lift(uid, modName, infraction.Id, infraction.Type),
		};
		context.ModeratorActions.Add(action);

		await context.SaveChangesAsync(cancellationToken);

		if (infraction.Type == InfractionType.Ban)
		{
			await cache.ExpireAsync(UserBanMiddleware.CacheKey(infraction.UserId), token: cancellationToken);
		}

		return TypedResults.Ok();
	}
}
