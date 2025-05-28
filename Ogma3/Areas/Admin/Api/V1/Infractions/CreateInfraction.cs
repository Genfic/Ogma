using Immediate.Apis.Shared;
using Immediate.Handlers.Shared;
using Immediate.Validations.Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.Extensions.Caching.Memory;
using Ogma3.Data;
using Ogma3.Data.Infractions;
using Ogma3.Data.ModeratorActions;
using Ogma3.Infrastructure.Constants;
using Ogma3.Infrastructure.CustomValidators;
using Ogma3.Infrastructure.Extensions;
using Ogma3.Infrastructure.Middleware;
using Ogma3.Infrastructure.ServiceRegistrations;
using Ogma3.Services.UserService;

namespace Ogma3.Areas.Admin.Api.V1.Infractions;

using ReturnType = Results<UnauthorizedHttpResult, CreatedAtRoute<InfractionDto>>;

[Handler]
[MapPost("admin/api/infractions")]
[Authorize(AuthorizationPolicies.RequireAdminOrModeratorRole)]
public static partial class CreateInfraction
{
	[Validate]
	public sealed partial record Command
	(
		long UserId,
		[property: NotEmpty] string Reason,
		[property: Future] DateTimeOffset EndDate,
		InfractionType Type
	) : IValidationTarget<Command>;

	private static async ValueTask<ReturnType> HandleAsync(
		Command request,
		ApplicationDbContext context,
		IUserService userService,
		IMemoryCache cache,
		CancellationToken cancellationToken
	)
	{
		if (userService.User?.GetNumericId() is not {} uid) return TypedResults.Unauthorized();
		if (userService.User?.GetUsername() is not {} modName) return TypedResults.Unauthorized();

		var (userId, reason, dateTime, type) = request;
		var infraction = new Infraction
		{
			IssuedById = uid,
			UserId = userId,
			Reason = reason,
			ActiveUntil = dateTime,
			Type = type,
		};
		context.Infractions.Add(infraction);

		var action = new ModeratorAction
		{
			StaffMemberId = uid,
			Description = ModeratorActionTemplates.Infractions.Create(uid, modName, infraction.Id, reason, type),
		};
		context.ModeratorActions.Add(action);

		await context.SaveChangesAsync(cancellationToken);

		if (infraction.Type == InfractionType.Ban)
		{
			cache.Set(UserBanMiddleware.CacheKey(infraction.UserId), infraction.ActiveUntil);
		}

		return TypedResults.CreatedAtRoute(
			infraction.MapToResult(),
			nameof(GetInfractionDetails),
			new GetInfractionDetails.Query(infraction.Id)
		);
	}
}