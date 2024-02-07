using System.Threading;
using System.Threading.Tasks;
using Mediator;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Ogma3.Data;
using Ogma3.Data.Quotes;
using Ogma3.Infrastructure.MediatR.Bases;

namespace Ogma3.Api.V1.Quotes.Queries;

public static class GetRandom
{
	public sealed record Query : IRequest<ActionResult<QuoteDto>>;

	// ReSharper disable once SuggestBaseTypeForParameterInConstructor | DI errors out if base type is used, since only the derived one is registered
	public sealed class Handler(ApplicationDbContext context) : BaseHandler, IRequestHandler<Query, ActionResult<QuoteDto>>
	{
		public async ValueTask<ActionResult<QuoteDto>> Handle(Query request, CancellationToken cancellationToken)
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