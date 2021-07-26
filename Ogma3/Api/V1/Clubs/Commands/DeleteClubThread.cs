using System;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Ogma3.Data;
using Ogma3.Data.Clubs;
using Ogma3.Infrastructure.Constants;
using Ogma3.Infrastructure.Extensions;
using Ogma3.Services.UserService;

namespace Ogma3.Api.V1.Clubs.Commands
{
    public static class DeleteClubThread
    {
        public sealed record Query(long ThreadId) : IRequest<ActionResult<bool>>;

        public class Handler : IRequestHandler<Query, ActionResult<bool>>
        {
            private readonly ApplicationDbContext _context;
            private readonly ClaimsPrincipal _user;

            public Handler(ApplicationDbContext context, IUserService userService)
            {
                _context = context;
                _user = userService?.User;
            }

            public async Task<ActionResult<bool>> Handle(Query request, CancellationToken cancellationToken)
            {
                var uid = _user.GetNumericId();
                if (uid is null) return new UnauthorizedResult();

                bool canDelete;
                if (_user.IsInRole(RoleNames.Admin) || _user.IsInRole(RoleNames.Moderator))
                {
                    canDelete = true;
                }
                else
                {
                    var roles = new[] { EClubMemberRoles.Founder, EClubMemberRoles.Admin, EClubMemberRoles.Moderator };

                    canDelete = await _context.ClubThreads
                        .Where(ct => ct.Id == request.ThreadId)
                        .Select(ct => ct.Club.ClubMembers
                            .Where(cm => cm.MemberId == uid)
                            .Any(cm => roles.Contains(cm.Role)))
                        .FirstOrDefaultAsync(cancellationToken);
                }

                if (!canDelete) return new UnauthorizedResult();

                var topic = await _context.ClubThreads
                    .Where(ct => ct.Id == request.ThreadId)
                    .FirstOrDefaultAsync(cancellationToken);
                
                topic.DeletedAt = DateTime.Now;
                var res = await _context.SaveChangesAsync(cancellationToken);

                return res > 0;
            }
        }
    }
}