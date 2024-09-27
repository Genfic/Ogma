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

		var isMember = await context.ClubMembers
			.Where(cm => cm.MemberId == uid)
			.Where(cm => cm.ClubId == request.ClubId)
			.AnyAsync(cancellationToken);

		if (isMember) return TypedResults.Ok(true);


		context.ClubMembers.Add(new ClubMember
		{
			ClubId = request.ClubId,
			MemberId = uid,
			Role = EClubMemberRoles.User,
		});

		await context.SaveChangesAsync(cancellationToken);

		return TypedResults.Ok(true);
	}
}