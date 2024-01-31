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

	public class Handler(ApplicationDbContext context) : BaseHandler, IRequestHandler<Query, ActionResult<QuoteDto>>
	{
		public async Task<ActionResult<QuoteDto>> Handle(Query request, CancellationToken cancellationToken)
		{
			var quote = await context.Database.SqlQueryRaw<QuoteDto>("""
			    SELECT q."Author", q."Body"
			    FROM "Quotes" q
			    TABLESAMPLE bernoulli(.5)
			    LIMIT 1
			    """)
				.FirstOrDefaultAsync(cancellationToken);

			return quote is null ? NotFound() : Ok(quote);
		}
	}
}