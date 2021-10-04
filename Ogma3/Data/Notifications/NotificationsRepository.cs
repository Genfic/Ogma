#nullable enable

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Routing;

namespace Ogma3.Data.Notifications;

public class NotificationsRepository
{
    private readonly ApplicationDbContext _context;
    private readonly IUrlHelper _urlHelper;

    public NotificationsRepository(ApplicationDbContext context, IUrlHelperFactory urlHelperFactory, IActionContextAccessor actionContextAccessor)
    {
        _context = context;
        _urlHelper = urlHelperFactory.GetUrlHelper(actionContextAccessor.ActionContext);
    }

    public async Task Create(ENotificationEvent @event, IEnumerable<long> recipientIds, string page, object routeData, string? fragment = null, string? body = null)
    {
        var notification = new Notification
        {
            Body = body,
            Event = @event,
            Url = _urlHelper.Page(page, routeData) + (fragment is null ? string.Empty : $"#{fragment}")
        };
        _context.Notifications.Add(notification);

        var notificationRecipients = recipientIds
            .Select(u => new NotificationRecipients { RecipientId = u, Notification = notification });
            
        _context.NotificationRecipients.AddRange(notificationRecipients);

        await _context.SaveChangesAsync();
    }
}