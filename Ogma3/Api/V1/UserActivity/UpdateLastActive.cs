using Immediate.Apis.Shared;
using Immediate.Handlers.Shared;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using Ogma3.Data;
using Ogma3.Services.UserService;

namespace Ogma3.Api.V1.UserActivity;

using ReturnType = Results<Ok<int>, NoContent>;

[Handler]
[MapMethod("HEAD", "api/useractivity")]
public sealed partial class UpdateLastActive(ApplicationDbContext context, IUserService userService)
{
	internal static void CustomizeEndpoint(IEndpointConventionBuilder endpoint)
		=> endpoint
			.DisableAntiforgery()
			.WithName(nameof(UpdateLastActive));

	private async ValueTask<ReturnType> HandleAsync(
		Command _,
		CancellationToken cancellationToken
	)
	{
		if (userService.UserId is not {} uid) return TypedResults.NoContent();

		var rows = await context.Users
			.TagWith(nameof(UpdateLastActive))
			.Where(u => u.Id == uid)
			.ExecuteUpdateAsync(
				setPropertyCalls: setters => setters.SetProperty(propertyExpression: u => u.LastActive, DateTimeOffset.UtcNow),
				cancellationToken);

		return TypedResults.Ok(rows);
	}

	[UsedImplicitly]
	public sealed record Command;
}