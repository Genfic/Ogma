using Microsoft.EntityFrameworkCore;
using Ogma3.Services;

namespace Ogma3.Data.Notifications;

public sealed class NotificationsRepository
(
	ApplicationDbContext context,
	CommentRedirector redirector
)
{
	public async Task Create(
		ENotificationEvent @event,
		IEnumerable<long> recipientIds,
		string url,
		string? body = null
	)
	{
		var notification = new Notification
		{
			Body = body,
			Event = @event,
			Url = url,
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
				redirection,
				body
			);
		}

		await context.SaveChangesAsync(cancellationToken);
	}
}