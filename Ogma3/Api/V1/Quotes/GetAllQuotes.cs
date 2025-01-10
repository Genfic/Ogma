using Immediate.Apis.Shared;
using Immediate.Handlers.Shared;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using Ogma3.Data;
using Ogma3.Data.Quotes;

namespace Ogma3.Api.V1.Quotes;

[Handler]
[MapGet("api/quotes")]
public static partial class GetAllQuotes
{
	internal static void CustomizeEndpoint(IEndpointConventionBuilder endpoint) => endpoint
		.WithName(nameof(GetAllQuotes));

	[UsedImplicitly]
	public sealed record Query;

	private static async ValueTask<Ok<List<FullQuoteDto>>> HandleAsync(Query _, ApplicationDbContext context, CancellationToken cancellationToken)
	{
		var quotes = await context.Quotes
			.OrderBy(q => q.Id)
			.ProjectToFullDto()
			.ToListAsync(cancellationToken);

		return TypedResults.Ok(quotes);
	}
	
}
