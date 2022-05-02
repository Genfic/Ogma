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
using Ogma3.Infrastructure.MediatR.Bases;
using Serilog;

namespace Ogma3.Api.V1.Quotes.Commands;

public static class CreateQuotesFromJson
{
	public sealed record Command(Stream Data) : IRequest<ActionResult<Response>>;

	public class CreateQuoteHandler : BaseHandler, IRequestHandler<Command, ActionResult<Response>>
	{
		private readonly ApplicationDbContext _context;

		public CreateQuoteHandler(ApplicationDbContext context)
		{
			_context = context;
		}

		public async Task<ActionResult<Response>> Handle(Command request, CancellationToken cancellationToken)
		{
			var data = await JsonSerializer.DeserializeAsync<IEnumerable<Quote>>(request.Data, cancellationToken: cancellationToken);
			if (data is null) return BadRequest();

			_context.Quotes.AddRange(data);
			try
			{
				var insertedRows = await _context.SaveChangesAsync(cancellationToken);
				return Ok(new Response(insertedRows));
			}
			catch (DbUpdateException ex)
			{
				Log.Error("Bulk Insert error in {Src}: {Msg}", ex.Source, ex.Message);
				return ServerError("Database Bulk Insert Error");
			}
		}
	}

	public sealed record Response(int InsertedRows);
}