using Immediate.Apis.Shared;
using Immediate.Handlers.Shared;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using Ogma3.Data;
using Ogma3.Infrastructure.Extensions;
using Ogma3.Services.UserService;

namespace Ogma3.Api.V1.Subscriptions;

using ReturnType = Results<UnauthorizedHttpResult, Ok<bool>>;

[Handler]
[MapGet("api/subscriptions/thread")]
public static partial class GetSubscriptionStatus
{
	public sealed record Query(long ThreadId);
	
	private static async ValueTask<ReturnType> HandleAsync(
		Query request,
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

		return TypedResults.Ok(isSubscribed);
	}
}