using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Ogma3.Data;
using Ogma3.Data.CommentsThreads;
using Ogma3.Infrastructure.Extensions;
using Ogma3.Services.UserService;

namespace Ogma3.Api.V1.Subscriptions.Commands
{
    public static class UnsubscribeCommentsThread
    {
        public sealed record Query(long ThreadId) : IRequest<ActionResult<bool>>;

        public class Handler : IRequestHandler<Query, ActionResult<bool>>
        {
            private readonly ApplicationDbContext _context;
            private readonly long? _uid;

            public Handler(ApplicationDbContext context, IUserService userService)
            {
                _context = context;
                _uid = userService?.User?.GetNumericId();
            }

            public async Task<ActionResult<bool>> Handle(Query request, CancellationToken cancellationToken)
            {
                if (_uid is null) return new UnauthorizedResult();

                var subscriber = await _context.CommentsThreadSubscribers
                    .Where(cts => cts.OgmaUserId == _uid)
                    .Where(cts => cts.CommentsThreadId == request.ThreadId)
                    .FirstOrDefaultAsync(cancellationToken);
                
                if (subscriber is null) return new OkObjectResult(false);

                _context.CommentsThreadSubscribers.Remove(subscriber);

                await _context.SaveChangesAsync(cancellationToken);

                return new OkObjectResult(false);
            }
        }
    }
}