using Immediate.Apis.Shared;
using Immediate.Handlers.Shared;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using Ogma3.Data;
using Ogma3.Data.InviteCodes;
using Ogma3.Services.UserService;

namespace Ogma3.Api.V1.InviteCodes;

using ReturnType = Results<UnauthorizedHttpResult, Ok<InviteCodeDto[]>>;

[Handler]
[MapGet("api/InviteCodes")]
[Authorize]
public sealed partial class GetIssuedInviteCodes(ApplicationDbContext context, IUserService userService)
{
	private async ValueTask<ReturnType> HandleAsync(
		Query _,
		CancellationToken cancellationToken
	)
	{
		if (userService.UserId is not {} uid) return TypedResults.Unauthorized();

		var codes = await context.InviteCodes
			.Where(ic => ic.IssuedById == uid)
			.OrderByDescending(ic => ic.IssueDate)
			.ProjectToDto()
			.ToArrayAsync(cancellationToken);

		return TypedResults.Ok(codes);
	}

	[UsedImplicitly]
	public sealed record Query;
}