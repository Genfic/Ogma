using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;
using Mediator;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Ogma3.Data;
using Ogma3.Data.Quotes;
using Ogma3.Infrastructure.MediatR.Bases;

namespace Ogma3.Api.V1.Quotes.Commands;

public static class CreateQuotesFromJson
{
	public sealed record Command(Stream Data) : IRequest<ActionResult<Response>>;

	public class CreateQuoteHandler(ApplicationDbContext context, ILogger<CreateQuoteHandler> logger)
		: BaseHandler, IRequestHandler<Command, ActionResult<Response>>
	{
		public async ValueTask<ActionResult<Response>> Handle(Command request, CancellationToken cancellationToken)
		{
			var data = await JsonSerializer.DeserializeAsync(request.Data, QuoteJsonContext.Default.QuoteArray, cancellationToken);
			if (data is null) return BadRequest();

			context.Quotes.AddRange(data);
			try
			{
				var insertedRows = await context.SaveChangesAsync(cancellationToken);
				return Ok(new Response(insertedRows));
			}
			catch (DbUpdateException ex)
			{
				logger.LogError("Bulk Insert error in {Src}: {Msg}", ex.Source, ex.Message);
				return ServerError("Database Bulk Insert Error");
			}
		}
	}

	public sealed record Response(int InsertedRows);
}

[JsonSerializable(typeof(Quote[]))]
[JsonSourceGenerationOptions(JsonSerializerDefaults.Web)]
public partial class QuoteJsonContext : JsonSerializerContext;