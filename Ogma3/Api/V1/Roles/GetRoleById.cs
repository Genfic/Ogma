using Immediate.Apis.Shared;
using Immediate.Handlers.Shared;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using Ogma3.Data;
using Ogma3.Data.Roles;

namespace Ogma3.Api.V1.Roles;

using ReturnType = Results<Ok<RoleDto>, NotFound>;

[Handler]
[MapGet("api/roles/{roleId:long}")]
public static partial class GetRoleById
{
	internal static void CustomizeEndpoint(IEndpointConventionBuilder endpoint) => endpoint.WithName(nameof(GetRoleById));

	public sealed record Query(long RoleId);

	private static async ValueTask<ReturnType> HandleAsync(Query request, ApplicationDbContext context, CancellationToken cancellationToken)
	{
		var role = await context.Roles
			.Where(r => r.Id == request.RoleId)
			.ProjectToDto()
			.FirstOrDefaultAsync(cancellationToken);

		return role is null ? TypedResults.NotFound() : TypedResults.Ok(role);
	}
}