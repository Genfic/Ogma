using Immediate.Apis.Shared;
using Immediate.Handlers.Shared;
using Immediate.Validations.Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using Ogma3.Data;
using Ogma3.Infrastructure.ServiceRegistrations;

namespace Ogma3.Api.V1.Ratings;

using ReturnType = Results<Ok<long>, NotFound>;

[Handler]
[MapGroup<ApiGroup>]
[MapDelete("ratings/{ratingId:long}")]
[Authorize(AuthorizationPolicies.RequireAdminRole)]
public sealed partial class DeleteRating(ApplicationDbContext context)
{
	internal static void CustomizeEndpoint(RouteHandlerBuilder endpoint)
		=> endpoint
			.ProducesValidationProblem();

	private async ValueTask<ReturnType> HandleAsync(
		Command request,
		CancellationToken cancellationToken
	)
	{
		var rows = await context.Ratings
			.Where(r => r.Id == request.RatingId)
			.ExecuteDeleteAsync(cancellationToken);

		return rows > 0 ? TypedResults.Ok(request.RatingId) : TypedResults.NotFound();
	}

	[Validate]
	public sealed partial record Command(long RatingId) : IValidationTarget<Command>;
}