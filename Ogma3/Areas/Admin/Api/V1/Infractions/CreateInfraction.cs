using Immediate.Apis.Shared;
using Immediate.Handlers.Shared;
using Immediate.Validations.Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Ogma3.Data;
using Ogma3.Data.Infractions;
using Ogma3.Infrastructure.CustomValidators;
using Ogma3.Infrastructure.ServiceRegistrations;
using Ogma3.Services;
using Ogma3.Services.ModeratorActionService;
using Ogma3.Services.UserService;

namespace Ogma3.Areas.Admin.Api.V1.Infractions;

using ReturnType = Results<UnauthorizedHttpResult, Ok, InternalServerError<string>>;

[Handler]
[MapPost("admin/api/infractions")]
[Authorize(AuthorizationPolicies.RequireAdminOrModeratorRole)]
public sealed partial class CreateInfraction(AppDbContext context, IUserService userService, BanCache cache, IModeratorActionService moderatorActionService)
{
	[Validate]
	public sealed partial record Command
	(
		long UserId,
		[property: NotEmpty] string Reason,
		[property: Future] DateTimeOffset EndDate,
		InfractionType Type
	) : IValidationTarget<Command>;

	private async ValueTask<ReturnType> HandleAsync(
		Command request,
		CancellationToken cancellationToken
	)
	{
		if (userService.UserId is not {} uid) return TypedResults.Unauthorized();

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

		moderatorActionService.LogInfractionCreated(userId, infraction.Id, reason, type);

		await context.SaveChangesAsync(cancellationToken);

		if (infraction.Type != InfractionType.Ban)
		{
			return TypedResults.Ok();
		}

		var res = await cache.Ban(userId, dateTime);

		return res ? TypedResults.Ok() : TypedResults.InternalServerError("Could not ban user!");
	}
}
