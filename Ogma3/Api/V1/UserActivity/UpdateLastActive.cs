using Immediate.Apis.Shared;
using Immediate.Handlers.Shared;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using Ogma3.Data;
using Ogma3.Infrastructure.Extensions;
using Ogma3.Services.UserService;

namespace Ogma3.Api.V1.UserActivity;

using ReturnType = Results<Ok<int>, NoContent>;

[Handler]
[MapMethod("api/useractivity", "HEAD")]
[Authorize]
public static partial class UpdateLastActive
{
	internal static void CustomizeEndpoint(IEndpointConventionBuilder endpoint) => endpoint
		.DisableAntiforgery()
		.WithName(nameof(UpdateLastActive));
	
	[UsedImplicitly]
	public sealed record Command;

	private static async ValueTask<ReturnType> HandleAsync(
		Command _,
		ApplicationDbContext context,
		IUserService userService,
		CancellationToken cancellationToken
	)
	{
		if (userService.User?.GetNumericId() is not {} uid) return TypedResults.NoContent();

		var rows = await context.Users
			.Where(u => u.Id == uid)
			.ExecuteUpdateAsync(
				setters => setters.SetProperty(u => u.LastActive, DateTime.Now.ToUniversalTime()),
				cancellationToken
			);

		return TypedResults.Ok(rows);
	}
}