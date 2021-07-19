using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using MediatR;

namespace Ogma3.Data.Quotes.Commands
{
    public static class CreateFromJson
    {
        public sealed record Command(Stream Data) : IRequest<bool>;
        
        public class CreateQuoteHandler : IRequestHandler<Command, bool>
        {
            private readonly ApplicationDbContext _context;

            public CreateQuoteHandler(ApplicationDbContext context)
            {
                _context = context;
            }

            public async Task<bool> Handle(Command request, CancellationToken cancellationToken)
            {
                var data = await JsonSerializer.DeserializeAsync<IEnumerable<Quote>>(request.Data, cancellationToken: cancellationToken);
                if (data is null) return false;

                _context.Quotes.AddRange(data);
                await _context.SaveChangesAsync(cancellationToken);
                return true;
            }
        }
        
    }
}