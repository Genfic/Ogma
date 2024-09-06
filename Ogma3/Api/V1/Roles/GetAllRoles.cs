using Immediate.Apis.Shared;
using Immediate.Handlers.Shared;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using Ogma3.Data;
using Ogma3.Data.Roles;

namespace Ogma3.Api.V1.Roles;

[Handler]
[MapGet("api/roles")]
public static partial class GetAllRoles
{
	public sealed record Query;
	
	private static async ValueTask<Ok<RoleDto[]>> HandleAsync(
		Query _,
		ApplicationDbContext context,
		CancellationToken cancellationToken
	)
	{
		var roles = await context.Roles
			.OrderByDescending(r => r.Order)
			.ProjectToDto()
			.ToArrayAsync(cancellationToken);

		return TypedResults.Ok(roles);
	}
}