using Immediate.Apis.Shared;
using Immediate.Handlers.Shared;
using Immediate.Validations.Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using Ogma3.Data;
using Ogma3.Services.UserService;

namespace Ogma3.Api.V1.Subscriptions;

using ReturnType = Results<UnauthorizedHttpResult, Ok<bool>>;

[Handler]
[Authorize]
[MapDelete("api/subscriptions/thread")]
public static partial class UnsubscribeCommentsThread
{
	[Validate]
	public sealed partial record Command(long ThreadId) : IValidationTarget<Command>;

	private static async ValueTask<ReturnType> HandleAsync(
		Command request,
		ApplicationDbContext context,
		IUserService userService,
		CancellationToken cancellationToken
	)
	{
		if (userService.UserId is not {} uid) return TypedResults.Unauthorized();

		var subscriber = await context.CommentThreadSubscribers
			.Where(cts => cts.OgmaUserId == uid)
			.Where(cts => cts.CommentsThreadId == request.ThreadId)
			.FirstOrDefaultAsync(cancellationToken);

		if (subscriber is null) return TypedResults.Ok(false);

		context.CommentThreadSubscribers.Remove(subscriber);

		await context.SaveChangesAsync(cancellationToken);

		return TypedResults.Ok(false);
	}
}