using Immediate.Apis.Shared;
using Immediate.Handlers.Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using Ogma3.Data;
using Ogma3.Data.CommentsThreads;
using Ogma3.Infrastructure.Extensions;
using Ogma3.Services.UserService;

namespace Ogma3.Api.V1.Subscriptions;

using ReturnType = Results<UnauthorizedHttpResult, Ok<bool>>;

[Handler]
[Authorize]
[MapPost("api/subscriptions/thread")]
public static partial class SubscribeCommentsThread
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

		var isSubscribed = await context.CommentsThreadSubscribers
			.Where(cts => cts.OgmaUserId == uid)
			.Where(cts => cts.CommentsThreadId == request.ThreadId)
			.AnyAsync(cancellationToken);

		if (isSubscribed) return TypedResults.Ok(true);

		context.CommentsThreadSubscribers.Add(new CommentsThreadSubscriber
		{
			OgmaUserId = uid,
			CommentsThreadId = request.ThreadId,
		});

		await context.SaveChangesAsync(cancellationToken);

		return TypedResults.Ok(true);
	}
}