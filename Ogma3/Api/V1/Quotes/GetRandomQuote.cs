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
public sealed partial class GetRandomQuote(ApplicationDbContext context)
{
	internal static void CustomizeEndpoint(IEndpointConventionBuilder endpoint)
		=> endpoint
			.RequireRateLimiting(RateLimiting.Quotes);

	private async ValueTask<ReturnType> HandleAsync(Query _, CancellationToken cancellationToken)
	{
		var quote = await context.Database.SqlQueryRaw<QuoteDto>(EmbeddedResourceQueries.GetRandomQoute_sql.LoadSql())
			.FirstOrDefaultAsync(cancellationToken);

		return quote is null ? TypedResults.NotFound() : TypedResults.Ok(quote);
	}

	[UsedImplicitly]
	public sealed record Query;
}