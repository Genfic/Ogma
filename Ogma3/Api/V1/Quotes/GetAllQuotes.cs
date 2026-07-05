using Immediate.Apis.Shared;
using Immediate.Handlers.Shared;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using Ogma3.Data;
using Ogma3.Data.Quotes;

namespace Ogma3.Api.V1.Quotes;

[Handler]
[MapGroup<ApiGroup>]
[MapGet("quotes")]
public sealed partial class GetAllQuotes(ApplicationDbContext context)
{
	internal static void CustomizeEndpoint(IEndpointConventionBuilder endpoint)
		=> endpoint
			.WithName(nameof(GetAllQuotes));

	private async ValueTask<Ok<List<FullQuoteDto>>> HandleAsync(Query _, CancellationToken cancellationToken)
	{
		var quotes = await context.Quotes
			.OrderBy(q => q.Id)
			.ProjectToFullDto()
			.ToListAsync(cancellationToken);

		return TypedResults.Ok(quotes);
	}

	[UsedImplicitly]
	public sealed record Query;
}