using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Ogma3.Data;
using Utils;

namespace Ogma3.Api.V1.SignIn.Queries
{
    public static class GetSignInData
    {
        public sealed record Query(string Name) : IRequest<ActionResult<Result>>;

        public class Handler : IRequestHandler<Query, ActionResult<Result>>
        {
            private readonly ApplicationDbContext _context;
            public Handler(ApplicationDbContext context) => _context = context;

            public async Task<ActionResult<Result>> Handle(Query request, CancellationToken cancellationToken)
            {
                var user = await _context.Users
                    .Where(u => u.NormalizedUserName == request.Name.Normalize().ToUpperInvariant())
                    .Select(u => new { u.Avatar, u.Title, u.Email })
                    .FirstOrDefaultAsync(cancellationToken);

                return user is null
                    ? new Result(Lorem.Picsum(200), string.Empty)
                    : new Result(user.Avatar ?? Lorem.Gravatar(user.Email), user.Title);
            }
        }

        public sealed record Result(string Avatar, string Title);
    }
}