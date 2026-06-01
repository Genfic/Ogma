using Immediate.Apis.Shared;
using Immediate.Handlers.Shared;
using Immediate.Validations.Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Ogma3.Data.Roles;
using Ogma3.Infrastructure.ServiceRegistrations;

namespace Ogma3.Api.V1.Roles;

using ReturnType = Results<Ok<long>, NotFound>;

[Handler]
[MapDelete("api/roles")]
[Authorize(AuthorizationPolicies.RequireAdminRole)]
public sealed partial class DeleteRole(RoleManager<OgmaRole> roleManager)
{
	internal static void CustomizeEndpoint(RouteHandlerBuilder endpoint)
		=> endpoint
			.ProducesValidationProblem();

	private async ValueTask<ReturnType> HandleAsync(
		Command request,
		CancellationToken cancellationToken
	)
	{
		cancellationToken.ThrowIfCancellationRequested();

		var role = await roleManager.FindByIdAsync(request.RoleId.ToString());

		if (role is null) return TypedResults.NotFound();

		await roleManager.DeleteAsync(role);

		return TypedResults.Ok(role.Id);
	}

	[Validate]
	public sealed partial record Command(long RoleId) : IValidationTarget<Command>;
}