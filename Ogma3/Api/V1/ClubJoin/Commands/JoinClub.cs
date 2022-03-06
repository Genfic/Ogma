using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Ogma3.Data;
using Ogma3.Data.Clubs;
using Ogma3.Infrastructure.Extensions;
using Ogma3.Infrastructure.MediatR.Bases;
using Ogma3.Services.UserService;

namespace Ogma3.Api.V1.ClubJoin.Commands;

public static class JoinClub
{
    public sealed record Command(long ClubId) : IRequest<ActionResult<bool>>;

    public class Handler : BaseHandler, IRequestHandler<Command, ActionResult<bool>>
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
            if (_uid is not {} uid) return Unauthorized();

            var isMember = await _context.ClubMembers
                .Where(cm => cm.MemberId == uid)
                .Where(cm => cm.ClubId == request.ClubId)
                .AnyAsync(cancellationToken);
                
            if (isMember) return Ok(true);

            var isBanned = await _context.ClubBans
                .Where(cb => cb.ClubId == request.ClubId)
                .Where(cb => cb.UserId == uid)
                .AnyAsync(cancellationToken);

            if (isBanned) return Unauthorized("You're banned from this club");
            
            _context.ClubMembers.Add(new ClubMember
            {
                ClubId = request.ClubId,
                MemberId = uid,
                Role = EClubMemberRoles.User
            });

            await _context.SaveChangesAsync(cancellationToken);
                    
            return Ok(true);
        }
    }
}