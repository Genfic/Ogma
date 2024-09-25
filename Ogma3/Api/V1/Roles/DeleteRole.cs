using Immediate.Apis.Shared;
using Immediate.Handlers.Shared;
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
public static partial class DeleteRole
{
	public sealed record Command(long RoleId);
	
	private static async ValueTask<ReturnType> HandleAsync(
		Command request,
		RoleManager<OgmaRole> roleManager,
		CancellationToken _
	)
	{
		var role = await roleManager.FindByIdAsync(request.RoleId.ToString());

		if (role is null) return TypedResults.NotFound();

		await roleManager.DeleteAsync(role);

		return TypedResults.Ok(role.Id);
	}
}