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
public static partial class UpdateLastActive
{
	internal static void CustomizeEndpoint(IEndpointConventionBuilder endpoint)
		=> endpoint
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
		if (userService.UserId is not {} uid) return TypedResults.NoContent();

		// var rows = await CompiledQuery(context, uid, cancellationToken);

		var rows = await context.Users
			.TagWith(nameof(UpdateLastActive))
			.Where(u => u.Id == uid)
			.ExecuteUpdateAsync(setters => setters.SetProperty(u => u.LastActive, DateTimeOffset.UtcNow), cancellationToken);

		return TypedResults.Ok(rows);
	}

	// private static readonly Func<ApplicationDbContext, long, CancellationToken, Task<int>> CompiledQuery =
	// 	EF.CompileAsyncQuery(static (ApplicationDbContext context, long uid, CancellationToken _)
	// 		=> context.Users
	// 			.TagWith(nameof(UpdateLastActive))
	// 			.Where(u => u.Id == uid)
	// 			.ExecuteUpdate(setters => setters.SetProperty(u => u.LastActive, DateTimeOffset.UtcNow)));
}