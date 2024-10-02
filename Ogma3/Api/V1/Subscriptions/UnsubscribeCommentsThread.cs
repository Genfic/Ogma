using Immediate.Apis.Shared;
using Immediate.Handlers.Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using Ogma3.Data;
using Ogma3.Infrastructure.Extensions;
using Ogma3.Services.UserService;

namespace Ogma3.Api.V1.Subscriptions;

using ReturnType = Results<UnauthorizedHttpResult, Ok<bool>>;

[Handler]
[Authorize]
[MapDelete("api/subscriptions/thread")]
public static partial class UnsubscribeCommentsThread
{
	public sealed record Command(long ThreadId);

	private static async ValueTask<ReturnType> HandleAsync(
		Command request,
		ApplicationDbContext context,
		IUserService userService,
		CancellationToken cancellationToken
	)
	{
		if (userService.User?.GetNumericId() is not {} uid) return TypedResults.Unauthorized();

		var subscriber = await context.CommentsThreadSubscribers
			.Where(cts => cts.OgmaUserId == uid)
			.Where(cts => cts.CommentsThreadId == request.ThreadId)
			.FirstOrDefaultAsync(cancellationToken);

		if (subscriber is null) return TypedResults.Ok(false);

		context.CommentsThreadSubscribers.Remove(subscriber);

		await context.SaveChangesAsync(cancellationToken);

		return TypedResults.Ok(false);
	}
}