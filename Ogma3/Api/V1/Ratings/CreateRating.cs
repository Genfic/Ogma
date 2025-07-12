using Immediate.Apis.Shared;
using Immediate.Handlers.Shared;
using Immediate.Validations.Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Ogma3.Data;
using Ogma3.Data.Ratings;
using Ogma3.Infrastructure.ServiceRegistrations;

namespace Ogma3.Api.V1.Ratings;

[Handler]
[MapPost("api/ratings")]
[Authorize(AuthorizationPolicies.RequireAdminRole)]
public static partial class CreateRating
{
	[Validate]
	public sealed partial record Command : IValidationTarget<Command>
	{
		[MinLength(CTConfig.Rating.MinNameLength)]
		[MaxLength(CTConfig.Rating.MaxNameLength)]
		public required string Name { get; init; }
		[MinLength(CTConfig.Rating.MinDescriptionLength)]
		[MaxLength(CTConfig.Rating.MaxDescriptionLength)]
		public required string Description { get; init; }
		public required bool BlacklistedByDefault { get; init; }
		public required byte Order { get; init; }

		[MinLength(3)]
		[MaxLength(6)]
		public required string Color { get; init; }
	}

	private static async ValueTask<CreatedAtRoute<RatingApiDto>> HandleAsync(
		Command request,
		ApplicationDbContext context,
		CancellationToken cancellationToken
	)
	{
		var rating = new Rating
		{
			Name = request.Name,
			Description = request.Description,
			BlacklistedByDefault = request.BlacklistedByDefault,
			Order = request.Order,
			Color = request.Color,
		};

		context.Ratings.Add(rating);

		await context.SaveChangesAsync(cancellationToken);

		return TypedResults.CreatedAtRoute(rating.MapToApiDto(), nameof(GetRatingById), new GetRatingById.Query(rating.Id));
	}
}