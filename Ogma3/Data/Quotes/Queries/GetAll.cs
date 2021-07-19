using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Ogma3.Data.Quotes.Queries
{
    public static class GetAll
    {
        public sealed record Query : IRequest<List<Quote>>;

        public class Handler : IRequestHandler<Query, List<Quote>>
        {
            private readonly ApplicationDbContext _context;

            public Handler(ApplicationDbContext context)
            {
                _context = context;
            }

            public async Task<List<Quote>> Handle(Query request, CancellationToken cancellationToken)
            {
                return await _context.Quotes.ToListAsync(cancellationToken);
            }
        }
    }
}