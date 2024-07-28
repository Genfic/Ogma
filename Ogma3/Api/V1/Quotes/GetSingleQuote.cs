using Immediate.Apis.Shared;
using Immediate.Handlers.Shared;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using Ogma3.Data;
using Ogma3.Data.Quotes;

namespace Ogma3.Api.V1.Quotes;

using ReturnType = Results<NotFound, Ok<QuoteDto>>;

[Handler]
[MapGet("api/quotes/{id:long}")]
public static partial class GetSingleQuote
{
	internal static void CustomizeEndpoint(IEndpointConventionBuilder endpoint)
		=> endpoint.WithName(nameof(GetSingleQuote));

	[UsedImplicitly]
	public sealed record Query(long Id);

	private static async ValueTask<ReturnType> HandleAsync(Query request, ApplicationDbContext context, CancellationToken cancellationToken)
	{
		var quote = await context.Quotes
			.Where(q => q.Id == request.Id)
			.ProjectToDto()
			.FirstOrDefaultAsync(cancellationToken);

		return quote is null ? TypedResults.NotFound() : TypedResults.Ok(quote);
	}
}