using Immediate.Apis.Shared;
using Immediate.Handlers.Shared;
using Immediate.Validations.Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Ogma3.Data.Roles;
using Ogma3.Infrastructure.ServiceRegistrations;

namespace Ogma3.Api.V1.Roles;

using ReturnType = Results<Conflict<string>, CreatedAtRoute<RoleDto>>;

[Handler]
[MapPost("api/roles")]
[Authorize(AuthorizationPolicies.RequireAdminRole)]
public static partial class CreateRole
{
	[Validate]
	public sealed partial record Command
	(
		[property: NotEmpty] string Name,
		bool IsStaff,
		string? Color,
		byte Order
	) : IValidationTarget<Command>;

	private static async ValueTask<ReturnType> HandleAsync(
		Command request,
		RoleManager<OgmaRole> roleManager,
		CancellationToken cancellationToken
	)
	{
		cancellationToken.ThrowIfCancellationRequested();
		if (await roleManager.RoleExistsAsync(request.Name)) return TypedResults.Conflict($"Role {request.Name} already exists");

		var role = new OgmaRole
		{
			Name = request.Name,
			IsStaff = request.IsStaff,
			Color = request.Color,
			Order = request.Order,
		};

		await roleManager.CreateAsync(role);

		return TypedResults.CreatedAtRoute(role.ToDto(), nameof(GetRoleById), new GetRoleById.Query(role.Id));
	}
}