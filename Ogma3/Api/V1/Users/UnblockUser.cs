using Immediate.Apis.Shared;
using Immediate.Handlers.Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Ogma3.Data;
using Ogma3.Infrastructure.Extensions;
using Ogma3.Services.UserService;

namespace Ogma3.Api.V1.Users;

using ReturnType = Results<UnauthorizedHttpResult, Ok<bool>, NotFound>;

[Handler]
[MapDelete("api/users/block")]
[Authorize]
public static partial class UnblockUser
{
	public sealed record Command(string Name);
	
	private static async ValueTask<ReturnType> HandleAsync(
		[FromBody] Command request,
		ApplicationDbContext context,
		IUserService userService,
		CancellationToken cancellationToken
	)
	{
		if (userService.User?.GetNumericId() is not {} uid) return TypedResults.Unauthorized();

		var targetUserId = await context.Users
			.Where(u => u.NormalizedUserName == request.Name.ToUpperInvariant().Normalize())
			.Select(u => (long?)u.Id)
			.FirstOrDefaultAsync(cancellationToken);

		if (targetUserId is null) return TypedResults.NotFound();

		var affectedRows = await context.BlockedUsers
			.Where(bu => bu.BlockingUserId == uid)
			.Where(bu => bu.BlockedUserId == targetUserId)
			.ExecuteDeleteAsync(cancellationToken);

		return affectedRows > 0 ? TypedResults.Ok(false) : TypedResults.NotFound();
	}
}