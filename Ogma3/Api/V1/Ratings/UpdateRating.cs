using Immediate.Apis.Shared;
using Immediate.Handlers.Shared;
using Immediate.Validations.Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using Ogma3.Data;
using Ogma3.Infrastructure.ServiceRegistrations;

namespace Ogma3.Api.V1.Ratings;

using ReturnType = Results<NotFound, Ok>;

[Handler]
[MapPut("api/ratings")]
[Authorize(AuthorizationPolicies.RequireAdminRole)]
public static partial class UpdateRating
{
	[Validate]
	public sealed partial record Command : IValidationTarget<Command>
	{
		public required long Id { get; init; }
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

	private static async ValueTask<ReturnType> HandleAsync(
		Command request,
		ApplicationDbContext context,
		CancellationToken cancellationToken
	)
	{
		var rows = await context.Ratings
			.Where(r => r.Id == request.Id)
			.ExecuteUpdateAsync(s => s
				.SetProperty(r => r.Name, request.Name)
				.SetProperty(r => r.Description, request.Description)
				.SetProperty(r => r.BlacklistedByDefault, request.BlacklistedByDefault)
				.SetProperty(r => r.Order, request.Order)
				.SetProperty(r => r.Color, request.Color), cancellationToken);

		return rows > 0 ? TypedResults.Ok() : TypedResults.NotFound();
	}
}