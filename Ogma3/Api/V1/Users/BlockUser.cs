using Immediate.Apis.Shared;
using Immediate.Handlers.Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using Ogma3.Data;
using Ogma3.Data.Users;
using Ogma3.Infrastructure.Extensions;
using Ogma3.Services.UserService;

namespace Ogma3.Api.V1.Users;

using ReturnType = Results<UnauthorizedHttpResult, Ok<bool>, NotFound>;

[Handler]
[MapPost("api/users/block")]
[Authorize]
public static partial class BlockUser
{
	public sealed record Command(string Name);
	
	private static async ValueTask<ReturnType> HandleAsync(
		Command request,
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
		
		if (targetUserId is not {} targetId) return TypedResults.NotFound();

		var exists = await context.BlockedUsers
			.Where(bu => bu.BlockingUserId == uid && bu.BlockedUserId == targetUserId)
			.AnyAsync(cancellationToken);

		if (exists) return TypedResults.Ok(true);

		context.BlockedUsers.Add(new UserBlock
		{
			BlockingUserId = uid,
			BlockedUserId = targetId,
		});
		await context.SaveChangesAsync(cancellationToken);

		return TypedResults.Ok(true);
	}

}