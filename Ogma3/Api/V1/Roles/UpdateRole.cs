using Immediate.Apis.Shared;
using Immediate.Handlers.Shared;
using Immediate.Validations.Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using Ogma3.Data;
using Ogma3.Infrastructure.ServiceRegistrations;
using Ogma3.Services.ModeratorActionService;

namespace Ogma3.Api.V1.Roles;

using ReturnType = Results<Ok, NotFound>;

[Handler]
[MapGroup<ApiGroup>]
[MapPut("roles")]
[Authorize(AuthorizationPolicies.RequireAdminRole)]
public sealed partial class UpdateRole(AppDbContext context, IModeratorActionService moderatorActionService)
{
	internal static void CustomizeEndpoint(RouteHandlerBuilder endpoint)
		=> endpoint
			.ProducesValidationProblem();

	private async ValueTask<ReturnType> HandleAsync(
		Command request,
		CancellationToken cancellationToken
	)
	{
		var role = await context.Roles
			.Where(r => r.Id == request.Id)
			.Select(r => new { r.Id, r.Name, r.IsStaff })
			.FirstOrDefaultAsync(cancellationToken);

		if (role is null)
		{
			return TypedResults.NotFound();
		}

		var rows = await context.Roles
			.Where(r => r.Id == request.Id)
			.ExecuteUpdateAsync(setPropertyCalls: setters => setters
					.SetProperty(propertyExpression: r => r.Name, request.Name)
					.SetProperty(propertyExpression: r => r.Order, request.Order)
					.SetProperty(propertyExpression: r => r.IsStaff, request.IsStaff)
					.SetProperty(propertyExpression: r => r.Color, request.Color),
				cancellationToken);

		if (rows > 0)
		{
			moderatorActionService.LogRoleUpdated(role.Id, role.Name, role.IsStaff);
		}

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