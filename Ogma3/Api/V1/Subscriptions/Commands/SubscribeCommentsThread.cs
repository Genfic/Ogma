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
    public static class SubscribeCommentsThread
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

                var isSubscribed = await _context.CommentsThreadSubscribers
                    .Where(cts => cts.OgmaUserId == _uid)
                    .Where(cts => cts.CommentsThreadId == request.ThreadId)
                    .AnyAsync(cancellationToken);
                
                if (isSubscribed) return new OkObjectResult(true);
                
                _context.CommentsThreadSubscribers.Add(new CommentsThreadSubscriber
                {
                    OgmaUserId = (long)_uid,
                    CommentsThreadId = request.ThreadId
                });

                await _context.SaveChangesAsync(cancellationToken);

                return new OkObjectResult(true);
            }
        }
    }
}