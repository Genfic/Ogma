using Immediate.Apis.Shared;
using Immediate.Handlers.Shared;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using Ogma3.Data;
using Ogma3.Data.Quotes;
using Ogma3.Infrastructure.ServiceRegistrations;

namespace Ogma3.Api.V1.Quotes;

using ReturnType = Results<NotFound, Ok<QuoteDto>>;

[Handler]
[MapGet("api/quotes/random")]
public static partial class GetRandomQuote
{
	internal static void CustomizeEndpoint(IEndpointConventionBuilder endpoint) => endpoint
		.RequireRateLimiting(RateLimiting.Quotes);

	[UsedImplicitly]
	public sealed record Query;
	
	private static async ValueTask<ReturnType> HandleAsync(Query _, ApplicationDbContext context, CancellationToken cancellationToken)
	{
		var quote = await context.Database.SqlQueryRaw<QuoteDto>("""
		    SELECT q."Author", q."Body"
		    FROM "Quotes" q
		    TABLESAMPLE SYSTEM_ROWS(1)
		    LIMIT 1
		    """)
			.FirstOrDefaultAsync(cancellationToken);

		return quote is null ? TypedResults.NotFound() : TypedResults.Ok(quote);
	}
}