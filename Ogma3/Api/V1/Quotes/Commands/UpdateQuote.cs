using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Ogma3.Data;
using Ogma3.Data.Quotes;
using Ogma3.Infrastructure.ActionResults;
using Serilog;

namespace Ogma3.Api.V1.Quotes.Commands
{
    public static class UpdateQuote
    {
        public sealed record Command(long Id, string Body, string Author) : IRequest<ActionResult<Quote>>;
        
        public class CreateQuoteHandler : IRequestHandler<Command, ActionResult<Quote>>
        {
            private readonly ApplicationDbContext _context;

            public CreateQuoteHandler(ApplicationDbContext context)
            {
                _context = context;
            }

            public async Task<ActionResult<Quote>> Handle(Command request, CancellationToken cancellationToken)
            {
                var (id, body, author) = request;
                var quote = new Quote
                {
                    Id = id,
                    Body = body,
                    Author = author
                };
                _context.Entry(quote).State = EntityState.Modified;
                try
                {
                    await _context.SaveChangesAsync(cancellationToken);
                }
                catch (DbUpdateException ex)
                {
                    Log.Error("Update error in {Src}: {Msg}", ex.Source, ex.Message);
                    return new ServerErrorResult("Database Update Error");
                }

                return new OkObjectResult(quote);
            }
        }
    }
}