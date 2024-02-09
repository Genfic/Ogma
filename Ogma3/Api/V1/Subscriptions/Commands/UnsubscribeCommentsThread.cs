using System.Linq;
using System.Threading;
using System.Threading.Tasks;
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

	public class Handler : BaseHandler, IRequestHandler<Command, ActionResult<bool>>
	{
		private readonly ApplicationDbContext _context;
		private readonly long? _uid;

		public Handler(ApplicationDbContext context, IUserService userService)
		{
			_context = context;
			_uid = userService.User?.GetNumericId();
		}

		public async ValueTask<ActionResult<bool>> Handle(Command request, CancellationToken cancellationToken)
		{
			if (_uid is null) return Unauthorized();

			var subscriber = await _context.CommentsThreadSubscribers
				.Where(cts => cts.OgmaUserId == _uid)
				.Where(cts => cts.CommentsThreadId == request.ThreadId)
				.FirstOrDefaultAsync(cancellationToken);

			if (subscriber is null) return Ok(false);

			_context.CommentsThreadSubscribers.Remove(subscriber);

			await _context.SaveChangesAsync(cancellationToken);

			return Ok(false);
		}
	}
}