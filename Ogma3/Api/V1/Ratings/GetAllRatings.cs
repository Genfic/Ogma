using Immediate.Apis.Shared;
using Immediate.Handlers.Shared;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using Ogma3.Data;
using Ogma3.Data.Ratings;

namespace Ogma3.Api.V1.Ratings;

using ReturnType = Ok<RatingApiDto[]>;

[Handler]
[MapGroup<ApiGroup>]
[MapGet("ratings")]
public sealed partial class GetAllRatings(ApplicationDbContext context)
{

	private async ValueTask<ReturnType> HandleAsync(
		Query _,
		CancellationToken cancellationToken
	)
	{
		var ratings = await context.Ratings
			.OrderBy(r => r.Order)
			.Select(RatingMapper.ToApiDto)
			.ToArrayAsync(cancellationToken);

		return TypedResults.Ok(ratings);
	}

	public sealed record Query;
}