using Immediate.Apis.Shared;
using Immediate.Handlers.Shared;
using Immediate.Validations.Shared;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using Ogma3.Data;
using Ogma3.Data.Roles;

namespace Ogma3.Api.V1.Roles;

using ReturnType = Results<Ok<RoleDto>, NotFound>;

[Handler]
[MapGroup<ApiGroup>]
[MapGet("roles/{roleId:long}")]
public sealed partial class GetRoleById(ApplicationDbContext context)
{
	internal static void CustomizeEndpoint(IEndpointConventionBuilder endpoint)
		=> endpoint
			.WithName(nameof(GetRoleById))
			.ProducesValidationProblem();

	private async ValueTask<ReturnType> HandleAsync(Query request, CancellationToken cancellationToken)
	{
		var role = await context.Roles
			.Where(r => r.Id == request.RoleId)
			.ProjectToDto()
			.FirstOrDefaultAsync(cancellationToken);

		return role is null ? TypedResults.NotFound() : TypedResults.Ok(role);
	}

	[Validate]
	public sealed partial record Query(long RoleId) : IValidationTarget<Query>;
}