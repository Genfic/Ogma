using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Ogma3.Data;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Ogma3.Api.V1.Comments.Queries
{
    public static class GetCommentMarkdown
    {
        public sealed record Query(long Id) : IRequest<ActionResult<string>>;

        public class Handler : IRequestHandler<Query, ActionResult<string>>
        {
            private readonly ApplicationDbContext _context;
            public Handler(ApplicationDbContext context) => _context = context;

            public async Task<ActionResult<string>> Handle(Query request, CancellationToken cancellationToken)
                => await _context.Comments
                    .Where(c => c.Id == request.Id)
                    .Select(c => c.Body)
                    .FirstOrDefaultAsync(cancellationToken);
        }
    }
}