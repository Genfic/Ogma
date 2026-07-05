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
[MapGroup<ApiGroup>]
[MapPut("ratings")]
[Authorize(AuthorizationPolicies.RequireAdminRole)]
public sealed partial class UpdateRating(ApplicationDbContext context)
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
			.Where(r => r.Id == request.Id)
			.ExecuteUpdateAsync(setPropertyCalls: s => s
				.SetProperty(propertyExpression: r => r.Name, request.Name)
				.SetProperty(propertyExpression: r => r.Description, request.Description)
				.SetProperty(propertyExpression: r => r.BlacklistedByDefault, request.BlacklistedByDefault)
				.SetProperty(propertyExpression: r => r.Order, request.Order)
				.SetProperty(propertyExpression: r => r.Color, request.Color), cancellationToken);

		return rows > 0 ? TypedResults.Ok() : TypedResults.NotFound();
	}

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
}