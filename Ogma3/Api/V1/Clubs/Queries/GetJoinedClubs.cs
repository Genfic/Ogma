using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Ogma3.Data;
using Ogma3.Infrastructure.Extensions;
using Ogma3.Services.UserService;

namespace Ogma3.Api.V1.Clubs.Queries
{
    public static class GetJoinedClubs
    {
        public sealed record Query : IRequest<ActionResult<List<Response>>>;

        public class Handler : IRequestHandler<Query, ActionResult<List<Response>>>
        {
            private readonly ApplicationDbContext _context;
            private readonly long? _uid;

            public Handler(ApplicationDbContext context, IUserService userService)
            {
                _context = context;
                _uid = userService?.User?.GetNumericId();
            }

            public async Task<ActionResult<List<Response>>> Handle(Query request, CancellationToken cancellationToken)
            {
                if (_uid is null) return new UnauthorizedResult();

                var clubs = await _context.Clubs
                    .Where(c => c.ClubMembers.Any(cm => cm.MemberId == (long)_uid))
                    .OrderBy(c => c.Name)
                    .Select(c => new Response(c.Id, c.Name, c.Icon))
                    .ToListAsync(cancellationToken);

                return new OkObjectResult(clubs);
            }
        }

        public sealed record Response(long Id, string Name, string Icon);
    }
}