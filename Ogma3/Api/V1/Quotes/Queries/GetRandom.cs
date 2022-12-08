using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Ogma3.Data;
using Ogma3.Data.Quotes;
using Ogma3.Infrastructure.MediatR.Bases;

namespace Ogma3.Api.V1.Quotes.Queries;

public static class GetRandom
{
	public sealed record Query : IRequest<ActionResult<QuoteDto>>;

	public class Handler : BaseHandler, IRequestHandler<Query, ActionResult<QuoteDto>>
	{
		private readonly ApplicationDbContext _context;

		public Handler(ApplicationDbContext context)
		{
			_context = context;
		}

		// TODO: Dapper..?
		public async Task<ActionResult<QuoteDto>> Handle(Query request, CancellationToken cancellationToken)
		{
			var quote = await _context.Quotes
				.FromSqlRaw("""
						SELECT *
						FROM "Quotes"
						OFFSET floor(random() * (
						    SELECT count(*)
						    FROM "Quotes"
						))
						LIMIT 1
					""")
				.AsNoTracking()
				.Select(q => new QuoteDto
				{
					Author = q.Author,
					Body = q.Body
				})
				.FirstOrDefaultAsync(cancellationToken);

			return quote is null ? NotFound() : Ok(quote);
		}
	}
}