using Microsoft.EntityFrameworkCore;
using Ogma3.Services;

namespace Ogma3.Data.Notifications;

public sealed class NotificationsRepository
(
	ApplicationDbContext context,
	LinkGenerator linkGenerator,
	CommentRedirector redirector
)
{
	public async Task Create(
		ENotificationEvent @event,
		IEnumerable<long> recipientIds,
		string page,
		object routeData,
		string? fragment = null,
		string? body = null
	)
	{
		var url = linkGenerator.GetPathByPage(page, values: routeData);

		var notification = new Notification
		{
			Body = body,
			Event = @event,
			Url = url + (fragment is null ? string.Empty : $"#{fragment}"),
		};
		context.Notifications.Add(notification);

		var notificationRecipients = recipientIds
			.Select(u => new NotificationRecipients { RecipientId = u, Notification = notification });

		context.NotificationRecipients.AddRange(notificationRecipients);

		await context.SaveChangesAsync();
	}

	public async Task NotifyUsers(long threadId, long commentId, string body, CancellationToken cancellationToken, long[]? except = null)
	{
		var subscribers = await context.CommentThreadSubscribers
			.Where(cts => cts.CommentsThreadId == threadId)
			.Select(cts => cts.OgmaUserId)
			.ToListAsync(cancellationToken);

		var redirection = await redirector.RedirectToComment(commentId);
		if (redirection is not null)
		{
			await Create(ENotificationEvent.WatchedThreadNewComment,
				subscribers.Except(except ?? []),
				redirection.Url,
				redirection.Params,
				redirection.Fragment,
				body
			);
		}

		await context.SaveChangesAsync(cancellationToken);
	}
}