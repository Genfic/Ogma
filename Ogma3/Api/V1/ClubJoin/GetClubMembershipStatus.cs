using Immediate.Apis.Shared;
using Immediate.Handlers.Shared;
using Immediate.Validations.Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using Ogma3.Data;
using Ogma3.Services.UserService;

namespace Ogma3.Api.V1.ClubJoin;

using ReturnType = Ok<bool>;

[Handler]
[MapGroup<ApiGroup>]
[MapGet("ClubJoin/{clubId:long}")]
[Authorize]
public sealed partial class GetClubMembershipStatus(ApplicationDbContext context, IUserService userService)
{
	internal static void CustomizeEndpoint(RouteHandlerBuilder endpoint)
		=> endpoint
			.ProducesValidationProblem();

	private async ValueTask<ReturnType> HandleAsync(
		Query request,
		CancellationToken cancellationToken
	)
	{
		if (userService.UserId is not {} uid) return TypedResults.Ok(false);

		var isMember = await context.ClubMembers
			.Where(cm => cm.ClubId == request.ClubId)
			.Where(cm => cm.MemberId == uid)
			.AnyAsync(cancellationToken);

		return TypedResults.Ok(isMember);
	}

	[Validate]
	public sealed partial record Query(long ClubId) : IValidationTarget<Query>;
}