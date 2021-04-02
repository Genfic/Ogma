using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Ogma3.Data.Notifications;
using Ogma3.Infrastructure.Extensions;

namespace Ogma3.Api.V1
{
    [Route("api/[controller]", Name = nameof(NotificationsController))]
    [ApiController]
    public class NotificationsController : ControllerBase
    {
        private readonly NotificationsRepository _notificationsRepo;

        public NotificationsController(NotificationsRepository notificationsRepo)
        {
            _notificationsRepo = notificationsRepo;
        }

        [HttpGet]
        [Authorize]
        public async Task<ActionResult<List<Notification>>> GetUserNotifications()
        {
            var uid = User.GetNumericId();
            if (uid is null) return Unauthorized();
            return await _notificationsRepo.GetForUser((long) uid);
        }

        [HttpGet("count")]
        [Authorize]
        public async Task<ActionResult<int>> CountUserNotifications()
        {
            var uid = User.GetNumericId();
            if (uid is null) return Unauthorized();
            return await _notificationsRepo.CountForUser((long) uid);
        }

        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> DeleteAsync(long id)
        {
            var uid = User.GetNumericId();
            if (uid is null) return Unauthorized();
            await _notificationsRepo.Delete(id, (long) uid);
            return Ok();
        }
    }
}