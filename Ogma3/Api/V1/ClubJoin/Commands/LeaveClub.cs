using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Ogma3.Data;
using Ogma3.Infrastructure.Extensions;
using Ogma3.Services.UserService;

namespace Ogma3.Api.V1.ClubJoin.Commands
{
    public static class LeaveClub
    {
        public sealed record Command(long ClubId) : IRequest<ActionResult<bool>>;

        public class Handler : IRequestHandler<Command, ActionResult<bool>>
        {            
            private readonly ApplicationDbContext _context;
            private readonly long? _uid;

            public Handler(ApplicationDbContext context, IUserService userService)
            {
                _context = context;
                _uid = userService?.User?.GetNumericId();
            }
            
            public async Task<ActionResult<bool>> Handle(Command request, CancellationToken cancellationToken)
            {
                if (_uid is null) return new UnauthorizedResult();

                var member = await _context.ClubMembers
                    .Where(cm => cm.MemberId == _uid)
                    .Where(cm => cm.ClubId == request.ClubId)
                    .FirstOrDefaultAsync(cancellationToken);
                
                if (member is null) return new OkObjectResult(false);
                
                _context.ClubMembers.Remove(member);
                await _context.SaveChangesAsync(cancellationToken);
                    
                return new OkObjectResult(false);
            }
        }
    }
}