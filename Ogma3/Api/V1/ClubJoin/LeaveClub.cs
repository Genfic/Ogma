using Immediate.Apis.Shared;
using Immediate.Handlers.Shared;
using Immediate.Validations.Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Ogma3.Data;
using Ogma3.Data.Clubs;
using Ogma3.Services.UserService;

namespace Ogma3.Api.V1.ClubJoin;

using ReturnType = Results<UnauthorizedHttpResult, Ok<bool>, BadRequest<string>>;

[Handler]
[MapDelete("api/clubjoin")]
[Authorize]
public static partial class LeaveClub
{
	[Validate]
	public sealed partial record Command(long ClubId) : IValidationTarget<Command>;

	private static async ValueTask<ReturnType> HandleAsync(
		[FromBody] Command request,
		ApplicationDbContext context,
		IUserService userService,
		CancellationToken cancellationToken
	)
	{
		if (userService.UserId is not {} uid) return TypedResults.Unauthorized();

		var isFounder = await context.ClubMembers
			.Where(cm => cm.MemberId == uid)
			.Where(cm => cm.ClubId == request.ClubId)
			.Where(cm => cm.Role == EClubMemberRoles.Founder)
			.AnyAsync(cancellationToken);

		if (isFounder) return TypedResults.BadRequest("Founder cannot leave their club. Delete it instead.");

		var member = await context.ClubMembers
			.Where(cm => cm.MemberId == uid)
			.Where(cm => cm.ClubId == request.ClubId)
			.FirstOrDefaultAsync(cancellationToken);

		if (member is null) return TypedResults.Ok(false);

		context.ClubMembers.Remove(member);
		await context.SaveChangesAsync(cancellationToken);

		return TypedResults.Ok(false);
	}
}