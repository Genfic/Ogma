using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Ogma3.Data.Quotes.Commands
{
    public static class Update
    {
        public sealed record Command(long Id, string Body, string Author) : IRequest<Response>;
        
        public class CreateQuoteHandler : IRequestHandler<Command, Response>
        {
            private readonly ApplicationDbContext _context;

            public CreateQuoteHandler(ApplicationDbContext context)
            {
                _context = context;
            }

            public async Task<Response> Handle(Command request, CancellationToken cancellationToken)
            {
                var (id, body, author) = request;
                var quote = new Quote
                {
                    Id = id,
                    Body = body,
                    Author = author
                };
                _context.Entry(quote).State = EntityState.Modified;
                await _context.SaveChangesAsync(cancellationToken);
                return new Response(quote.Id, quote.Body, quote.Author);
            }
        }
        
        public sealed record Response(long Id, string Body, string Author);
    }
}