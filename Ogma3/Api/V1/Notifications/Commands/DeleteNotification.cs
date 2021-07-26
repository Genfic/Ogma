using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Ogma3.Data;
using Ogma3.Infrastructure.Extensions;
using Ogma3.Services.UserService;

namespace Ogma3.Api.V1.Notifications.Commands
{
    public static class DeleteNotification
    {
        public sealed record Query(long NotificationId) : IRequest<IActionResult>;

        public class Handler : IRequestHandler<Query, IActionResult>
        {
            private readonly ApplicationDbContext _context;
            private readonly long? _uid;

            public Handler(ApplicationDbContext context, IUserService userService)
            {
                _context = context;
                _uid = userService?.User?.GetNumericId();
            }
            
            public async Task<IActionResult> Handle(Query request, CancellationToken cancellationToken)
            {
                if (_uid is null) return new UnauthorizedResult();
            
                var notificationRecipient = await _context.NotificationRecipients
                    .Where(nr => nr.RecipientId == (long) _uid)
                    .Where(nr => nr.NotificationId == request.NotificationId)
                    .FirstOrDefaultAsync(cancellationToken);
                
                if (notificationRecipient is null) return new NotFoundResult();
            
                _context.NotificationRecipients.Remove(notificationRecipient);
                
                await _context.SaveChangesAsync(cancellationToken);

                return new OkResult();

            }
        }
    }
}