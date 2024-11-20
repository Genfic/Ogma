using Immediate.Apis.Shared;
using Immediate.Handlers.Shared;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using Ogma3.Data;
using Ogma3.Data.Ratings;

namespace Ogma3.Api.V1.Ratings;

using ReturnType = Ok<RatingApiDto[]>;

[Handler]
[MapGet("api/ratings")]
public static partial class GetAllRatings
{
	public sealed record Query;

	private static async ValueTask<ReturnType> HandleAsync(
		Query _,
		ApplicationDbContext context,
		CancellationToken cancellationToken
	)
	{
		var ratings = await context.Ratings
			.OrderBy(r => r.Order)
			.ProjectToApiDto()
			.ToArrayAsync(cancellationToken);

		return TypedResults.Ok(ratings);
	}
}