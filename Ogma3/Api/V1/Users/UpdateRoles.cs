using Immediate.Apis.Shared;
using Immediate.Handlers.Shared;
using Immediate.Validations.Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using Ogma3.Data;
using Ogma3.Data.Users;
using Ogma3.Infrastructure.ServiceRegistrations;
using Ogma3.Services.ModeratorActionService;
using Ogma3.Services.UserService;

namespace Ogma3.Api.V1.Users;

using ReturnType = Results<UnauthorizedHttpResult, Ok, NotFound, StatusCodeHttpResult>;

[Handler]
[MapGroup<ApiGroup>]
[MapPost("users/roles")]
[Authorize(AuthorizationPolicies.RequireAdminRole)]
public sealed partial class UpdateRoles(AppDbContext context, IUserService userService, IModeratorActionService moderatorActionService)
{
	internal static void CustomizeEndpoint(RouteHandlerBuilder endpoint)
		=> endpoint
			.ProducesValidationProblem();

	private async ValueTask<ReturnType> HandleAsync(
		Command request,
		CancellationToken cancellationToken
	)
	{
		if (userService.UserId is null) return TypedResults.Unauthorized();

		var user = await context.Users
			.Where(u => u.Id == request.UserId)
			.Select(u => new
			{
				u.Id,
				u.UserName,
				Roles = u.Roles.Select(r => r.Id).ToArray(),
			})
			.FirstOrDefaultAsync(cancellationToken);

		if (user is null) return TypedResults.NotFound();

		var oldRoles = user.Roles;

		await context.UserRoles
			.Where(r => r.UserId == user.Id)
			.ExecuteDeleteAsync(cancellationToken);

		context.UserRoles
			.AddRange(request.Roles.Select(r => new UserRole
			{
				UserId = user.Id,
				RoleId = r,
			}));

		moderatorActionService.LogUserRolesChanged(user.Id, user.UserName, oldRoles, request.Roles.ToArray());

		await context.SaveChangesAsync(cancellationToken);

		return TypedResults.Ok();
	}

	[Validate]
	public sealed partial record Command(long UserId, List<long> Roles) : IValidationTarget<Command>;
}