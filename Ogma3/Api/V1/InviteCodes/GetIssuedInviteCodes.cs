using Immediate.Apis.Shared;
using Immediate.Handlers.Shared;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using Ogma3.Data;
using Ogma3.Data.InviteCodes;
using Ogma3.Infrastructure.Extensions;
using Ogma3.Services.UserService;

namespace Ogma3.Api.V1.InviteCodes;

using ReturnType = Results<UnauthorizedHttpResult, Ok<InviteCodeDto[]>>;

[Handler]
[MapGet("api/InviteCodes")]
[Authorize]
public static partial class GetIssuedInviteCodes
{
	[UsedImplicitly]
	public sealed record Query;

	private static async ValueTask<ReturnType> HandleAsync(
		Query _,
		ApplicationDbContext context,
		IUserService userService,
		CancellationToken cancellationToken
	)
	{
		if (userService.User?.GetNumericId() is not {} uid) return TypedResults.Unauthorized();

		var codes = await context.InviteCodes
			.Where(ic => ic.IssuedById == uid)
			.OrderByDescending(ic => ic.IssueDate)
			.ProjectToDto()
			.ToArrayAsync(cancellationToken);

		return TypedResults.Ok(codes);
	}
}