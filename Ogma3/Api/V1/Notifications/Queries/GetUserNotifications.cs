using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Ogma3.Data;
using Ogma3.Data.Notifications;
using Ogma3.Infrastructure.Extensions;
using Ogma3.Infrastructure.MediatR.Bases;
using Ogma3.Services.UserService;

namespace Ogma3.Api.V1.Notifications.Queries;

public static class GetUserNotifications
{
	public sealed record Query : IRequest<ActionResult<List<Result>>>;

	public class Handler : BaseHandler, IRequestHandler<Query, ActionResult<List<Result>>>
	{
		private readonly ApplicationDbContext _context;
		private readonly long? _uid;

		public Handler(ApplicationDbContext context, IUserService userService)
		{
			_context = context;
			_uid = userService?.User?.GetNumericId();
		}

		public async Task<ActionResult<List<Result>>> Handle(Query request, CancellationToken cancellationToken)
		{
			if (_uid is null) return Unauthorized();
			var notifications = await _context.NotificationRecipients
				.Where(nr => nr.RecipientId == (long)_uid)
				.Select(nr => nr.Notification)
				.Select(n => new Result(n.Id, n.Body, n.Url, n.DateTime, n.Event))
				.ToListAsync(cancellationToken);

			return Ok(notifications);
		}
	}

	public record Result(long Id, string Body, string Url, DateTime DateTime, ENotificationEvent Event)
	{
		public string Message => Event switch
		{
			ENotificationEvent.System => "[SYSTEM]",
			ENotificationEvent.WatchedStoryUpdated => "The story you're watching just updated.",
			ENotificationEvent.WatchedThreadNewComment => "The comments thread you're following has a new comment.",
			ENotificationEvent.FollowedAuthorNewBlogpost => "The author you're following just wrote a new blogpost.",
			ENotificationEvent.FollowedAuthorNewStory => "The author you're following just created a new story.",
			ENotificationEvent.CommentReply => "One of your comments just got a reply.",
			_ => throw new ArgumentOutOfRangeException(nameof(Event), Event, null)
		};
	}
}