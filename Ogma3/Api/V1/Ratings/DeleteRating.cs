using Immediate.Apis.Shared;
using Immediate.Handlers.Shared;
using Immediate.Validations.Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using Ogma3.Data;
using Ogma3.Infrastructure.ServiceRegistrations;
using Ogma3.Services.ModeratorActionService;

namespace Ogma3.Api.V1.Ratings;

using ReturnType = Results<Ok<long>, NotFound>;

[Handler]
[MapGroup<ApiGroup>]
[MapDelete("ratings/{ratingId:long}")]
[Authorize(AuthorizationPolicies.RequireAdminRole)]
public sealed partial class DeleteRating(AppDbContext context, IModeratorActionService moderatorActionService)
{
	internal static void CustomizeEndpoint(RouteHandlerBuilder endpoint)
		=> endpoint
			.ProducesValidationProblem();

	private async ValueTask<ReturnType> HandleAsync(
		Command request,
		CancellationToken cancellationToken
	)
	{
		var rating = await context.Ratings
			.Where(r => r.Id == request.RatingId)
			.Select(r => new { r.Id, r.Name })
			.FirstOrDefaultAsync(cancellationToken);

		if (rating is null)
		{
			return TypedResults.NotFound();
		}

		var rows = await context.Ratings
			.Where(r => r.Id == request.RatingId)
			.ExecuteDeleteAsync(cancellationToken);

		if (rows > 0)
		{
			moderatorActionService.LogRatingDeleted(rating.Id, rating.Name);
		}

		return rows > 0 ? TypedResults.Ok(request.RatingId) : TypedResults.NotFound();
	}

	[Validate]
	public sealed partial record Command(long RatingId) : IValidationTarget<Command>;
}