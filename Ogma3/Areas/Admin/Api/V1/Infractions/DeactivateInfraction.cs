using Immediate.Apis.Shared;
using Immediate.Handlers.Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using Ogma3.Data;
using Ogma3.Data.ModeratorActions;
using Ogma3.Infrastructure.Constants;
using Ogma3.Infrastructure.Extensions;
using Ogma3.Infrastructure.ServiceRegistrations;
using Ogma3.Services.UserService;

namespace Ogma3.Areas.Admin.Api.V1.Infractions;

using ReturnType = Results<Ok, UnauthorizedHttpResult, NotFound>;

[Handler]
[MapDelete("admin/api/infractions/{infractionId:long}")]
[Authorize(Policy = AuthorizationPolicies.RequireAdminOrModeratorRole)]
public static partial class DeactivateInfraction
{
	public sealed record Command(long InfractionId);

	private static async ValueTask<ReturnType> HandleAsync(
		Command request,
		ApplicationDbContext context,
		IUserService userService,
		CancellationToken cancellationToken
	)
	{
		if (userService.User?.GetNumericId() is not {} uid) return TypedResults.Unauthorized();
		if (userService.User?.GetUsername() is not {} modName) return TypedResults.Unauthorized();

		var infraction = await context.Infractions
			.Where(i => i.Id == request.InfractionId)
			.FirstOrDefaultAsync(cancellationToken);

		if (infraction is null) return TypedResults.NotFound();

		infraction.RemovedAt = DateTime.Now;
		infraction.RemovedById = uid;

		var action = new ModeratorAction
		{
			StaffMemberId = uid,
			Description = ModeratorActionTemplates.Infractions.Lift(uid, modName, infraction.Id, infraction.Type),
		};
		context.ModeratorActions.Add(action);

		await context.SaveChangesAsync(cancellationToken);

		return TypedResults.Ok();
	}
}