using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using LinqToDB;
using LinqToDB.Common;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Ogma3.Data;
using Ogma3.Infrastructure.Extensions;
using Ogma3.Services.UserService;

namespace Ogma3.Api.V1.ClubJoin.Queries
{
    public static class GetClubMembershipStatus
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
                if (_uid is null) return new OkObjectResult(false);
            
                var isMember = await _context.ClubMembers
                    .Where(cm => cm.ClubId == request.ClubId)
                    .Where(cm => cm.MemberId == _uid)
                    .AnyAsync(token: cancellationToken);
            
                return new OkObjectResult(isMember);
            }
        }
    }
}