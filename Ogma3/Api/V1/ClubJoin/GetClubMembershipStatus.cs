using Immediate.Apis.Shared;
using Immediate.Handlers.Shared;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using Ogma3.Data;
using Ogma3.Infrastructure.Extensions;
using Ogma3.Services.UserService;

namespace Ogma3.Api.V1.ClubJoin;

using ReturnType = Ok<bool>;

[Handler]
[MapGet("/api/clubjoin/{clubId:long}")]
[Authorize]
public static partial class GetClubMembershipStatus
{
	[UsedImplicitly]
	public sealed record Query(long ClubId);
	
	private static async ValueTask<ReturnType> HandleAsync(
		Query request,
		ApplicationDbContext context,
		IUserService userService,
		CancellationToken cancellationToken
	)
	{
		if (userService.User?.GetNumericId() is not {} uid) return TypedResults.Ok(false);

		var isMember = await context.ClubMembers
			.Where(cm => cm.ClubId == request.ClubId)
			.Where(cm => cm.MemberId == uid)
			.AnyAsync(cancellationToken);

		return TypedResults.Ok(isMember);
	}
}