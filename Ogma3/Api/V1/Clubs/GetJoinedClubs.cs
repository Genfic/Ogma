using Immediate.Apis.Shared;
using Immediate.Handlers.Shared;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using Ogma3.Data;
using Ogma3.Services.UserService;

namespace Ogma3.Api.V1.Clubs;

using ReturnType = Results<UnauthorizedHttpResult, Ok<GetJoinedClubs.Response[]>>;

[Handler]
[MapGroup<ApiGroup>]
[MapGet("clubs/user")]
public sealed partial class GetJoinedClubs(ApplicationDbContext context, IUserService userService)
{

	private async ValueTask<ReturnType> HandleAsync(
		Query _,
		CancellationToken cancellationToken
	)
	{
		if (userService.UserId is not {} uid) return TypedResults.Unauthorized();

		var clubs = await context.ClubMembers
			.Where(cm => cm.MemberId == uid)
			.OrderBy(cm => cm.Club.Name)
			.Select(cm => new Response(cm.ClubId, cm.Club.Name, cm.Club.Icon.Url))
			.ToArrayAsync(cancellationToken);

		return TypedResults.Ok(clubs);
	}

	[UsedImplicitly]
	public sealed record Query;

	public sealed record Response(long Id, string Name, string Icon);
}