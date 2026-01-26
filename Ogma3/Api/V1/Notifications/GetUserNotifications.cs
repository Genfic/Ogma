using Immediate.Apis.Shared;
using Immediate.Handlers.Shared;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using Ogma3.Data;
using Ogma3.Data.Notifications;
using Ogma3.Infrastructure.Exceptions;
using Ogma3.Services.UserService;

namespace Ogma3.Api.V1.Notifications;

using ReturnType = Results<Ok<List<GetUserNotifications.Result>>, UnauthorizedHttpResult>;

[Handler]
[MapGet("api/notifications")]
[Authorize]
public static partial class GetUserNotifications
{
	[UsedImplicitly]
	public sealed record Query;

	private static async ValueTask<ReturnType> HandleAsync(
		Query _,
		ApplicationDbContext context,
		IUserService userService,
		CancellationToken cancellationToken
	)
	{
		if (userService.UserId is not {} uid) return TypedResults.Unauthorized();

		var notifications = await context.NotificationRecipients
			.Where(nr => nr.RecipientId == uid)
			.Select(nr => nr.Notification)
			.Select(n => new Result(n.Id, n.Body, n.Url, n.DateTime, n.Event))
			.ToListAsync(cancellationToken);

		return TypedResults.Ok(notifications);
	}


	public sealed record Result(long Id, string? Body, string Url, DateTimeOffset DateTime, ENotificationEvent Event)
	{
		public string Message => Event switch
		{
			ENotificationEvent.System => "[SYSTEM]",
			ENotificationEvent.WatchedStoryUpdated => "The story you're watching just updated.",
			ENotificationEvent.WatchedThreadNewComment => "The comments thread you're following has a new comment.",
			ENotificationEvent.FollowedAuthorNewBlogpost => "The author you're following just wrote a new blogpost.",
			ENotificationEvent.FollowedAuthorNewStory => "The author you're following just created a new story.",
			ENotificationEvent.CommentReply => "One of your comments just got a reply.",
			_ => throw new UnexpectedEnumValueException<ENotificationEvent>(Event),
		};
	}
}