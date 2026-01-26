using Immediate.Apis.Shared;
using Immediate.Handlers.Shared;
using Immediate.Validations.Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using Ogma3.Data;
using Ogma3.Data.ModeratorActions;
using Ogma3.Data.Users;
using Ogma3.Infrastructure.Constants;
using Ogma3.Infrastructure.Extensions;
using Ogma3.Infrastructure.ServiceRegistrations;
using Ogma3.Services.UserService;

namespace Ogma3.Api.V1.Users;

using ReturnType = Results<UnauthorizedHttpResult, Ok, NotFound, StatusCodeHttpResult>;

[Handler]
[MapPost("api/users/roles")]
[Authorize(AuthorizationPolicies.RequireAdminRole)]
public static partial class UpdateRoles
{
	[Validate]
	public sealed partial record Command(long UserId, List<long> Roles) : IValidationTarget<Command>;

	private static async ValueTask<ReturnType> HandleAsync(
		Command request,
		ApplicationDbContext context,
		IUserService userService,
		CancellationToken cancellationToken
	)
	{
		if (userService.UserId is not {} uid) return TypedResults.Unauthorized();
		if (userService.User?.GetUsername() is not {} username) return TypedResults.Unauthorized();

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

		await context.UserRoles
			.Where(r => r.UserId == user.Id)
			.ExecuteDeleteAsync(cancellationToken);

		context.UserRoles
			.AddRange(request.Roles.Select(r => new UserRole
			{
				UserId = user.Id,
				RoleId = r,
			}));

		context.ModeratorActions.Add(new ModeratorAction
		{
			StaffMemberId = uid,
			Description = ModeratorActionTemplates.UserRolesChanged(user.UserName, user.Id, username, user.Roles, request.Roles.ToArray()),
		});

		await context.SaveChangesAsync(cancellationToken);


		return TypedResults.Ok();
	}
}