using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Ogma3.Data;
using Microsoft.EntityFrameworkCore;
using Ogma3.Data.Votes;
using Ogma3.Infrastructure.Extensions;
using Ogma3.Services.UserService;

namespace Ogma3.Api.V1.Votes.Commands
{
    public static class CreateVote
    {
        public sealed record Command(long StoryId) : IRequest<ActionResult<Result>>;

        public class Handler : IRequestHandler<Command, ActionResult<Result>>
        {
            private readonly ApplicationDbContext _context;
            private readonly long? _uid;

            public Handler(ApplicationDbContext context, IUserService userService)
            {
                _context = context;
                _uid = userService?.User?.GetNumericId();
            }
            
            public async Task<ActionResult<Result>> Handle(Command request, CancellationToken cancellationToken)
            {
                if (_uid is null) return new UnauthorizedResult();
                
                var didUserVote = await _context.Votes
                    .Where(v => v.StoryId == request.StoryId)
                    .Where(v => v.UserId == _uid)
                    .AnyAsync(cancellationToken);

                if (didUserVote) return new OkObjectResult(new Result(true));

                _context.Votes.Add(new Vote
                {
                    UserId = (long)_uid,
                    StoryId = request.StoryId
                });
                await _context.SaveChangesAsync(cancellationToken);

                var count = await _context.Votes
                    .Where(v => v.StoryId == request.StoryId)
                    .CountAsync(cancellationToken);

                return new OkObjectResult(new Result(true, count));
            }
        }

        public sealed record Result(bool DidVote, int? Count = null);
    }
}