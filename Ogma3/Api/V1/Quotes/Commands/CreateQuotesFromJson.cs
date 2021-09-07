using System.Collections.Generic;
using System.IO;
using System.Text.Json;
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
    public static class CreateQuotesFromJson
    {
        public sealed record Command(Stream Data) : IRequest<ActionResult<Response>>;
        
        public class CreateQuoteHandler : IRequestHandler<Command, ActionResult<Response>>
        {
            private readonly ApplicationDbContext _context;

            public CreateQuoteHandler(ApplicationDbContext context)
            {
                _context = context;
            }

            public async Task<ActionResult<Response>> Handle(Command request, CancellationToken cancellationToken)
            {
                var data = await JsonSerializer.DeserializeAsync<IEnumerable<Quote>>(request.Data, cancellationToken: cancellationToken);
                if (data is null) return new BadRequestResult();

                _context.Quotes.AddRange(data);
                try
                {
                    var insertedRows = await _context.SaveChangesAsync(cancellationToken);
                    return new OkObjectResult(new Response(insertedRows));
                }
                catch (DbUpdateException ex)
                {
                    Log.Error("Bulk Insert error in {Src}: {Msg}", ex.Source, ex.Message);
                    return new ServerErrorResult("Database Bulk Insert Error");
                }
            }
        }

        public sealed record Response(int InsertedRows);
    }
}