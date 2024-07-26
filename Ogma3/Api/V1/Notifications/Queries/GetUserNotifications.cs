using Mediator;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Ogma3.Data;
using Ogma3.Data.Notifications;
using Ogma3.Infrastructure.Exceptions;
using Ogma3.Infrastructure.Extensions;
using Ogma3.Infrastructure.Mediator.Bases;
using Ogma3.Services.UserService;

namespace Ogma3.Api.V1.Notifications.Queries;

public static class GetUserNotifications
{
	public sealed record Query : IRequest<ActionResult<List<Result>>>;

	public class Handler(ApplicationDbContext context, IUserService userService)
		: BaseHandler, IRequestHandler<Query, ActionResult<List<Result>>>
	{
		
		public async ValueTask<ActionResult<List<Result>>> Handle(Query request, CancellationToken cancellationToken)
		{
			if (userService.User?.GetNumericId() is not {} uid) return Unauthorized();
			var notifications = await context.NotificationRecipients
				.Where(nr => nr.RecipientId == uid)
				.Select(nr => nr.Notification)
				.Select(n => new Result(n.Id, n.Body, n.Url, n.DateTime, n.Event))
				.ToListAsync(cancellationToken);

			return Ok(notifications);
		}
	}

	public record Result(long Id, string? Body, string Url, DateTime DateTime, ENotificationEvent Event)
	{
		public string Message => Event switch
		{
			ENotificationEvent.System => "[SYSTEM]",
			ENotificationEvent.WatchedStoryUpdated => "The story you're watching just updated.",
			ENotificationEvent.WatchedThreadNewComment => "The comments thread you're following has a new comment.",
			ENotificationEvent.FollowedAuthorNewBlogpost => "The author you're following just wrote a new blogpost.",
			ENotificationEvent.FollowedAuthorNewStory => "The author you're following just created a new story.",
			ENotificationEvent.CommentReply => "One of your comments just got a reply.",
			_ => throw new UnexpectedEnumValueException<ENotificationEvent>(Event)
		};
	}
}