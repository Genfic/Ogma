using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Ogma3.Data;
using Ogma3.Data.Quotes;
using Ogma3.Infrastructure.ActionResults;
using Ogma3.Services;
using Serilog;

namespace Ogma3.Api.V1.Quotes.Commands
{
    public static class CreateQuote
    {
        public sealed record Command(string Body, string Author) : IRequest<ActionResult<Quote>>;

        public class CreateQuoteHandler : IRequestHandler<Command, ActionResult<Quote>>
        {
            private readonly ApplicationDbContext _context;

            public CreateQuoteHandler(ApplicationDbContext context)
            {
                _context = context;
            }

            public async Task<ActionResult<Quote>> Handle(Command request, CancellationToken cancellationToken)
            {
                var (body, author) = request;
                var quote = new Quote
                {
                    Body = body,
                    Author = author
                };
                _context.Quotes.Add(quote);

                try
                {
                    await _context.SaveChangesAsync(cancellationToken);
                }
                catch (DbUpdateException ex)
                {
                    Log.Error("Creation error in {Src}: {Msg}", ex.Source, ex.Message);
                    return new ServerErrorResult("Database Creation Error");
                }
                
                Jog.Log(new
                {
                    Action = nameof(QuotesController.GetQuote),
                    Controller = nameof(QuotesController),
                });

                return new CreatedAtActionResult(
                    nameof(QuotesController.GetQuote),
                    nameof(QuotesController)[..^10],
                    new { id = quote.Id },
                    new QuoteDto { Author = quote.Author, Body = quote.Body }
                );
            }
        }
    }
}