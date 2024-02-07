using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Mediator;
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

	public class Handler(ApplicationDbContext context, IUserService userService) : BaseHandler, IRequestHandler<Query, ActionResult<int>>
	{
		public async ValueTask<ActionResult<int>> Handle(Query request, CancellationToken cancellationToken)
		{
			if (userService.User?.GetNumericId() is not {} uid) return NotFound();

			var count = await context.NotificationRecipients
				.Where(nr => nr.RecipientId == uid)
				.CountAsync(cancellationToken);

			return Ok(count);
		}
	}
}