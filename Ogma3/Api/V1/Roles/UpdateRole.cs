using Immediate.Apis.Shared;
using Immediate.Handlers.Shared;
using Immediate.Validations.Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using Ogma3.Data;
using Ogma3.Infrastructure.ServiceRegistrations;

namespace Ogma3.Api.V1.Roles;

using ReturnType = Results<Ok, NotFound>;

[Handler]
[MapPut("api/roles")]
[Authorize(AuthorizationPolicies.RequireAdminRole)]
public sealed partial class UpdateRole(ApplicationDbContext context)
{
	internal static void CustomizeEndpoint(RouteHandlerBuilder endpoint)
		=> endpoint
			.ProducesValidationProblem();

	private async ValueTask<ReturnType> HandleAsync(
		Command request,
		CancellationToken cancellationToken
	)
	{
		var rows = await context.Roles
			.Where(r => r.Id == request.Id)
			.ExecuteUpdateAsync(setPropertyCalls: setters => setters
					.SetProperty(propertyExpression: r => r.Name, request.Name)
					.SetProperty(propertyExpression: r => r.Order, request.Order)
					.SetProperty(propertyExpression: r => r.IsStaff, request.IsStaff)
					.SetProperty(propertyExpression: r => r.Color, request.Color),
				cancellationToken);

		return rows > 0 ? TypedResults.Ok() : TypedResults.NotFound();
	}

	[Validate]
	public sealed partial record Command
	(
		long Id,
		[property: NotEmpty] string Name,
		bool IsStaff,
		string? Color,
		byte Order
	) : IValidationTarget<Command>;
}