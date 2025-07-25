using Immediate.Apis.Shared;
using Immediate.Handlers.Shared;
using Immediate.Validations.Shared;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using Ogma3.Data;
using Ogma3.Data.Ratings;

namespace Ogma3.Api.V1.Ratings;

using ReturnType = Results<NotFound, Ok<RatingApiDto>>;

[Handler]
[MapGet("api/ratings/{id:long}")]
public static partial class GetRatingById
{
	internal static void CustomizeEndpoint(IEndpointConventionBuilder endpoint)
		=> endpoint.WithName(nameof(GetRatingById));

	[Validate]
	public sealed partial record Query(long Id) : IValidationTarget<Query>;

	private static async ValueTask<ReturnType> HandleAsync(
		Query request,
		ApplicationDbContext context,
		CancellationToken cancellationToken
	)
	{
		var ratings = await context.Ratings
			.Where(r => r.Id == request.Id)
			.Select(RatingMapper.ToApiDto)
			.FirstOrDefaultAsync(cancellationToken);

		return ratings is null ? TypedResults.NotFound() : TypedResults.Ok(ratings);
	}
}