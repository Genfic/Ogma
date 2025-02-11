using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.EntityFrameworkCore;
using Ogma3.Services;

namespace Ogma3.Data.Notifications;

public sealed class NotificationsRepository
{
	private readonly ApplicationDbContext _context;
	private readonly IUrlHelper _urlHelper;
	private readonly CommentRedirector _redirector;

	public NotificationsRepository(
		ApplicationDbContext context, 
		IUrlHelperFactory urlHelperFactory,
		IActionContextAccessor actionContextAccessor, 
		CommentRedirector redirector
	) {
		if (actionContextAccessor is not { ActionContext: not null })
			throw new NullReferenceException(nameof(actionContextAccessor.ActionContext));
		
		_context = context;
		_redirector = redirector;
		_urlHelper = urlHelperFactory.GetUrlHelper(actionContextAccessor.ActionContext);
	}

	public async Task Create(ENotificationEvent @event, IEnumerable<long> recipientIds, string page, object routeData,
		string? fragment = null, string? body = null)
	{
		var notification = new Notification
		{
			Body = body,
			Event = @event,
			Url = _urlHelper.Page(page, routeData) + (fragment is null ? string.Empty : $"#{fragment}"),
		};
		_context.Notifications.Add(notification);

		var notificationRecipients = recipientIds
			.Select(u => new NotificationRecipients { RecipientId = u, Notification = notification });

		_context.NotificationRecipients.AddRange(notificationRecipients);

		await _context.SaveChangesAsync();
	}

	public async Task NotifyUsers(long threadId, long commentId, string body, CancellationToken cancellationToken, long[]? except = null)
	{
		var subscribers = await _context.CommentThreadSubscribers
			.Where(cts => cts.CommentsThreadId == threadId)
			.Select(cts => cts.OgmaUserId)
			.ToListAsync(cancellationToken);

		var redirection = await _redirector.RedirectToComment(commentId);
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

		await _context.SaveChangesAsync(cancellationToken);
	}
}