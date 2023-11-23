using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Ogma3.Data;
using Ogma3.Infrastructure.Extensions;
using Ogma3.Infrastructure.MediatR.Bases;
using Ogma3.Services.UserService;

namespace Ogma3.Api.V1.Notifications.Commands;

public static class DeleteNotification
{
	public sealed record Command(long NotificationId) : IRequest<ActionResult>;

	public class Handler : BaseHandler, IRequestHandler<Command, ActionResult>
	{
		private readonly ApplicationDbContext _context;
		private readonly long? _uid;

		public Handler(ApplicationDbContext context, IUserService userService)
		{
			_context = context;
			_uid = userService.User?.GetNumericId();
		}

		public async Task<ActionResult> Handle(Command request, CancellationToken cancellationToken)
		{
			if (_uid is null) return Unauthorized();

			var res = await _context.NotificationRecipients
				.Where(nr => nr.RecipientId == (long)_uid)
				.Where(nr => nr.NotificationId == request.NotificationId)
				.ExecuteDeleteAsync(cancellationToken);

			return res > 0 ? Ok() : NotFound();
		}
	}
}