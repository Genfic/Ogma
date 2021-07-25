using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Ogma3.Data;
using Ogma3.Data.Clubs;
using Ogma3.Infrastructure.Extensions;
using Ogma3.Services.UserService;

namespace Ogma3.Api.V1.ClubJoin.Commands
{
    public static class JoinClub
    {
        public sealed record Query(long ClubId) : IRequest<ActionResult<bool>>;

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

                var isMember = await _context.ClubMembers
                    .Where(cm => cm.MemberId == _uid)
                    .Where(cm => cm.ClubId == request.ClubId)
                    .AnyAsync(cancellationToken);
                
                if (isMember) return new OkObjectResult(true);
                
                _context.ClubMembers.Add(new ClubMember
                {
                    ClubId = request.ClubId,
                    MemberId = (long)_uid,
                    Role = EClubMemberRoles.User
                });

                await _context.SaveChangesAsync(cancellationToken);
                    
                return new OkObjectResult(true);
            }
        }
    }
}