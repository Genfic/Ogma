using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Ogma3.Data.Quotes.Queries;

namespace Ogma3.Data.Quotes.Commands
{
    public static class Delete
    {
        public sealed record Command(long Id) : IRequest<Response>;
        
        public class CreateQuoteHandler : IRequestHandler<Command, Response>
        {
            private readonly ApplicationDbContext _context;

            public CreateQuoteHandler(ApplicationDbContext context)
            {
                _context = context;
            }

            public async Task<Response> Handle(Command request, CancellationToken cancellationToken)
            {
                var quote = await _context.Quotes
                    .FirstOrDefaultAsync(q => q.Id == request.Id, cancellationToken);

                if (quote is null) return null;
                
                _context.Remove(quote);
                await _context.SaveChangesAsync(cancellationToken);

                return new Response(quote.Id, quote.Body, quote.Author);
            }
        }
        
        public sealed record Response(long Id, string Body, string Author);
    }
}