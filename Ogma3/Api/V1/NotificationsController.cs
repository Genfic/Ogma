using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Ogma3.Data;
using Ogma3.Data.Notifications;
using Ogma3.Infrastructure.Extensions;

namespace Ogma3.Api.V1
{
    [Route("api/[controller]", Name = nameof(NotificationsController))]
    [ApiController]
    public class NotificationsController : ControllerBase
    {
        private readonly NotificationsRepository _notificationsRepo;
        private readonly ApplicationDbContext _context;

        public NotificationsController(NotificationsRepository notificationsRepo, ApplicationDbContext context)
        {
            _notificationsRepo = notificationsRepo;
            _context = context;
        }

        [HttpGet]
        [Authorize]
        public async Task<ActionResult<List<Notification>>> GetUserNotifications()
        {
            var uid = User.GetNumericId();
            if (uid is null) return Unauthorized();
            return await _context.NotificationRecipients
                .Where(nr => nr.RecipientId == (long) uid)
                .Select(nr => nr.Notification)
                .ToListAsync();
        }

        [HttpGet("count")]
        [Authorize]
        public async Task<ActionResult<int>> CountUserNotifications()
        {
            var uid = User.GetNumericId();
            if (uid is null) return Unauthorized();
            return await _context.NotificationRecipients
                .Where(nr => nr.RecipientId == (long) uid)
                .CountAsync();
        }

        [HttpDelete("{id:long}")]
        [Authorize]
        public async Task<IActionResult> DeleteAsync(long id)
        {
            var uid = User.GetNumericId();
            if (uid is null) return Unauthorized();
            
            var notificationRecipient = await _context.NotificationRecipients
                .Where(nr => nr.RecipientId == (long) uid)
                .Where(nr => nr.NotificationId == id)
                .FirstOrDefaultAsync();
            if (notificationRecipient is null) return NotFound();
            
            _context.NotificationRecipients.Remove(notificationRecipient);
            await _context.SaveChangesAsync();

            return Ok();
        }
    }
}