using Immediate.Apis.Shared;
using Immediate.Handlers.Shared;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using Ogma3.Data;
using Ogma3.Data.Clubs;
using Ogma3.Infrastructure.Extensions;
using Ogma3.Services.UserService;

namespace Ogma3.Api.V1.ClubJoin;

using ReturnType = Results<UnauthorizedHttpResult, NotFound, Ok<bool>>;

[Handler]
[MapPost("api/clubjoin")]
[Authorize]
public static partial class JoinClub
{
	[UsedImplicitly]
	public sealed record Command(long ClubId);
	
	private static async ValueTask<ReturnType> HandleAsync(
		Command request,
		ApplicationDbContext context,
		IUserService userService,
		CancellationToken cancellationToken
	)
	{
		if (userService.User?.GetNumericId() is not {} uid) return TypedResults.Unauthorized();

		var memberOrBanned = await context.Clubs
			.Where(c => c.Id == request.ClubId)
			.Select(c => new MemberOrBanned(
				c.ClubMembers.Any(cm => cm.MemberId == uid),
				c.BannedUsers.Any(u => u.Id == uid)
			))
			.FirstOrDefaultAsync(cancellationToken);

		if (memberOrBanned is null) return TypedResults.NotFound();

		if (memberOrBanned.IsMember) return TypedResults.Ok(true);

		if (memberOrBanned.IsBanned) return TypedResults.Unauthorized();

		context.ClubMembers.Add(new ClubMember
		{
			ClubId = request.ClubId,
			MemberId = uid,
			Role = EClubMemberRoles.User,
		});

		await context.SaveChangesAsync(cancellationToken);

		return TypedResults.Ok(true);
	}

	private sealed record MemberOrBanned(bool IsMember, bool IsBanned);
}