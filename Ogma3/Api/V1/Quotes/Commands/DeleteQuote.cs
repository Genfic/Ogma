using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Ogma3.Data;
using Ogma3.Data.Quotes;
using Ogma3.Infrastructure.MediatR.Bases;
using Serilog;

namespace Ogma3.Api.V1.Quotes.Commands;

public static class DeleteQuote
{
    public sealed record Command(long Id) : IRequest<ActionResult<Quote>>;
        
    public class CreateQuoteHandler : BaseHandler, IRequestHandler<Command, ActionResult<Quote>>
    {
        private readonly ApplicationDbContext _context;

        public CreateQuoteHandler(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<ActionResult<Quote>> Handle(Command request, CancellationToken cancellationToken)
        {
            var quote = await _context.Quotes
                .FirstOrDefaultAsync(q => q.Id == request.Id, cancellationToken);

            if (quote is null) return NotFound();
                
            _context.Remove(quote);
            try
            {
                await _context.SaveChangesAsync(cancellationToken);
            }
            catch (DbUpdateException ex)
            {
                Log.Error("Delete error in {Src}: {Msg}", ex.Source, ex.Message);
                return ServerError("Database Delete Error");
            }

            return Ok(quote);
        }
    }
        
}