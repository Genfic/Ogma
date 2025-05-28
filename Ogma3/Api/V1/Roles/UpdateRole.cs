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
public static partial class UpdateRole
{
	[Validate]
	public sealed partial record Command
	(
		long Id,
		[property: NotEmpty] string Name,
		bool IsStaff,
		string Color,
		byte Order
	) : IValidationTarget<Command>;

	private static async ValueTask<ReturnType> HandleAsync(
		Command request,
		ApplicationDbContext context,
		CancellationToken cancellationToken
	)
	{
		var rows = await context.Roles
			.Where(r => r.Id == request.Id)
			.ExecuteUpdateAsync(setters => setters
					.SetProperty(r => r.Name, request.Name)
					.SetProperty(r => r.Order, request.Order)
					.SetProperty(r => r.IsStaff, request.IsStaff)
					.SetProperty(r => r.Color, request.Color),
				cancellationToken);

		return rows > 0 ? TypedResults.Ok() : TypedResults.NotFound();
	}
}