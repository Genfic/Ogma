using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Ogma3.Data;
using Ogma3.Infrastructure.Extensions;
using Ogma3.Infrastructure.MediatR.Bases;
using Ogma3.Services.UserService;

namespace Ogma3.Api.V1.Votes.Commands;

public static class DeleteVote
{
    public sealed record Command(long StoryId) : IRequest<ActionResult<Result>>;

    public class Handler : BaseHandler, IRequestHandler<Command, ActionResult<Result>>
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
            if (_uid is null) return Unauthorized();
                
            var vote = await _context.Votes
                .Where(v => v.StoryId == request.StoryId)
                .Where(v => v.UserId == _uid)
                .FirstOrDefaultAsync(cancellationToken);

            if (vote is null) return Ok(new Result(false));

            _context.Votes.Remove(vote);
            await _context.SaveChangesAsync(cancellationToken);

            var count = await _context.Votes
                .Where(v => v.StoryId == request.StoryId)
                .CountAsync(cancellationToken);

            return Ok(new Result(false, count));
        }
    }

    public sealed record Result(bool DidVote, int? Count = null);
}