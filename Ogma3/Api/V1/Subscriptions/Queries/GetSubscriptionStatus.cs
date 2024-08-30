using Mediator;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Ogma3.Data;
using Ogma3.Infrastructure.Extensions;
using Ogma3.Infrastructure.Mediator.Bases;
using Ogma3.Services.UserService;

namespace Ogma3.Api.V1.Subscriptions.Queries;

public static class GetSubscriptionStatus
{
	public sealed record Query(long ThreadId) : IRequest<ActionResult<bool>>;

	public class Handler(ApplicationDbContext context, IUserService userService) : BaseHandler, IRequestHandler<Query, ActionResult<bool>>
	{
		private readonly long? _uid = userService.User?.GetNumericId();

		public async ValueTask<ActionResult<bool>> Handle(Query request, CancellationToken cancellationToken)
		{
			var isSubscribed = await context.CommentsThreadSubscribers
				.Where(cts => cts.OgmaUserId == _uid)
				.Where(cts => cts.CommentsThreadId == request.ThreadId)
				.AnyAsync(cancellationToken);

			return Ok(isSubscribed);
		}
	}
}