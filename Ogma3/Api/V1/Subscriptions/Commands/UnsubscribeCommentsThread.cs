using Mediator;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Ogma3.Data;
using Ogma3.Infrastructure.Extensions;
using Ogma3.Infrastructure.Mediator.Bases;
using Ogma3.Services.UserService;

namespace Ogma3.Api.V1.Subscriptions.Commands;

public static class UnsubscribeCommentsThread
{
	public sealed record Command(long ThreadId) : IRequest<ActionResult<bool>>;

	public sealed class Handler(ApplicationDbContext context, IUserService userService) : BaseHandler, IRequestHandler<Command, ActionResult<bool>>
	{
		private readonly long? _uid = userService.User?.GetNumericId();

		public async ValueTask<ActionResult<bool>> Handle(Command request, CancellationToken cancellationToken)
		{
			if (_uid is null) return Unauthorized();

			var subscriber = await context.CommentsThreadSubscribers
				.Where(cts => cts.OgmaUserId == _uid)
				.Where(cts => cts.CommentsThreadId == request.ThreadId)
				.FirstOrDefaultAsync(cancellationToken);

			if (subscriber is null) return Ok(false);

			context.CommentsThreadSubscribers.Remove(subscriber);

			await context.SaveChangesAsync(cancellationToken);

			return Ok(false);
		}
	}
}