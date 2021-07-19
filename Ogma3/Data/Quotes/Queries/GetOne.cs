using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Ogma3.Data.Quotes.Queries
{
    public static class GetOne
    {
        public sealed record Query(long Id) : IRequest<Response>;

        public class Handler : IRequestHandler<Query, Response>
        {
            private readonly ApplicationDbContext _context;

            public Handler(ApplicationDbContext context)
            {
                _context = context;
            }

            public async Task<Response> Handle(Query request, CancellationToken cancellationToken)
            {
                return await _context.Quotes
                    .Where(q => q.Id == request.Id)
                    .Select(q => new Response(q.Body, q.Author))
                    .FirstOrDefaultAsync(cancellationToken);
            }
        }
        
        public sealed record Response(string Body, string Author);
    }
}