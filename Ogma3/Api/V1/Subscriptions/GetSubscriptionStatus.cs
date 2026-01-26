using Immediate.Apis.Shared;
using Immediate.Handlers.Shared;
using Immediate.Validations.Shared;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using Ogma3.Data;
using Ogma3.Services.UserService;

namespace Ogma3.Api.V1.Subscriptions;

using ReturnType = Results<UnauthorizedHttpResult, Ok<bool>>;

[Handler]
[MapGet("api/subscriptions/thread")]
public static partial class GetSubscriptionStatus
{
	[Validate]
	public sealed partial record Query(long ThreadId) : IValidationTarget<Query>;

	private static async ValueTask<ReturnType> HandleAsync(
		Query request,
		ApplicationDbContext context,
		IUserService userService,
		CancellationToken cancellationToken
	)
	{
		if (userService.UserId is not {} uid) return TypedResults.Unauthorized();

		var isSubscribed = await context.CommentThreadSubscribers
			.Where(cts => cts.OgmaUserId == uid)
			.Where(cts => cts.CommentsThreadId == request.ThreadId)
			.AnyAsync(cancellationToken);

		return TypedResults.Ok(isSubscribed);
	}
}