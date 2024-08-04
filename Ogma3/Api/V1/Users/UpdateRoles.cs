using Immediate.Apis.Shared;
using Immediate.Handlers.Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using Ogma3.Data;
using Ogma3.Data.ModeratorActions;
using Ogma3.Infrastructure.Comparers;
using Ogma3.Infrastructure.Constants;
using Ogma3.Infrastructure.Extensions;
using Ogma3.Infrastructure.ServiceRegistrations;
using Ogma3.Services.UserService;
using Serilog;

namespace Ogma3.Api.V1.Users;

using ReturnType = Results<UnauthorizedHttpResult, Ok, NotFound, StatusCodeHttpResult>;

[Handler]
[MapPost("api/users/roles")]
[Authorize(Policy = AuthorizationPolicies.RequireAdminRole)]
public static partial class UpdateRoles
{
	public sealed record Command(long UserId, IEnumerable<long> Roles);

	private static async ValueTask<ReturnType> HandleAsync(
		Command request,
		ApplicationDbContext context,
		IUserService userService,
		CancellationToken cancellationToken
	)
	{
		if (userService.User?.GetNumericId() is not {} uid) return TypedResults.Unauthorized();
		if (userService.User?.GetUsername() is not {} username) return TypedResults.Unauthorized();

		var user = await context.Users
			.Where(u => u.Id == request.UserId)
			.Include(u => u.Roles)
			.FirstOrDefaultAsync(cancellationToken);

		if (user is null) return TypedResults.NotFound();

		var newRoles = await context.OgmaRoles
			.Where(ur => request.Roles.Contains(ur.Id))
			.ToListAsync(cancellationToken);

		// Handle role removal
		var removedRoles = user.Roles.Except(newRoles, new OgmaRoleComparer()).ToList();
		foreach (var role in removedRoles)
		{
			user.Roles.Remove(role);
		}

		context.ModeratorActions.AddRange(removedRoles.Select(r => new ModeratorAction
		{
			StaffMemberId = uid,
			Description = ModeratorActionTemplates.UserRoleRemoved(user, username, r.Name)
		}));

		// Handle role adding
		var addedRoles = newRoles.Except(user.Roles, new OgmaRoleComparer()).ToList();
		foreach (var role in addedRoles)
		{
			user.Roles.Add(role);
		}

		context.ModeratorActions.AddRange(addedRoles.Select(r => new ModeratorAction
		{
			StaffMemberId = uid,
			Description = ModeratorActionTemplates.UserRoleAdded(user, username, r.Name),
		}));

		try
		{
			await context.SaveChangesAsync(cancellationToken);
		}
		catch (Exception e)
		{
			Log.Error(e, "Exception occurred when staff member {Staff} tried adding roles {Role} to user {User}", uid, request.Roles, request.UserId);
			return TypedResults.StatusCode(StatusCodes.Status500InternalServerError);
		}

		return TypedResults.Ok();
	}
}