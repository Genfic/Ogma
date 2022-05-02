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

namespace Ogma3.Api.V1.Notifications.Queries;

public static class CountUserNotifications
{
	public sealed record Query : IRequest<ActionResult<int>>;

	public class Handler : BaseHandler, IRequestHandler<Query, ActionResult<int>>
	{
		private readonly ApplicationDbContext _context;
		private readonly long? _uid;

		public Handler(ApplicationDbContext context, IUserService userService)
		{
			_context = context;
			_uid = userService?.User?.GetNumericId();
		}

		public async Task<ActionResult<int>> Handle(Query request, CancellationToken cancellationToken)
		{
			if (_uid is null) return Unauthorized();

			var count = await _context.NotificationRecipients
				.Where(nr => nr.RecipientId == (long)_uid)
				.CountAsync(cancellationToken);

			return Ok(count);
		}
	}
}