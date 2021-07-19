using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Ogma3.Data.Quotes.Queries;

namespace Ogma3.Data.Quotes.Commands
{
    public static class Create
    {
        public sealed record Command(string Body, string Author) : IRequest<Response>;
        
        public class CreateQuoteHandler : IRequestHandler<Command, Response>
        {
            private readonly ApplicationDbContext _context;

            public CreateQuoteHandler(ApplicationDbContext context)
            {
                _context = context;
            }

            public async Task<Response> Handle(Command request, CancellationToken cancellationToken)
            {
                var (body, author) = request;
                var quote = new Quote
                {
                    Body = body,
                    Author = author
                };
                _context.Quotes.Add(quote);
                await _context.SaveChangesAsync(cancellationToken);
                return new Response(quote.Id, quote.Body, quote.Author);
            }
        }
        
        public sealed record Response(long Id, string Body, string Author);
    }
}