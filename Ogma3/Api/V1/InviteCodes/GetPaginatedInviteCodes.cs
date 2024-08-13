using Immediate.Apis.Shared;
using Immediate.Handlers.Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using Ogma3.Data;
using Ogma3.Data.InviteCodes;
using Ogma3.Infrastructure.ServiceRegistrations;

namespace Ogma3.Api.V1.InviteCodes;

using ReturnType = Ok<InviteCodeDto[]>;

[Handler]
[MapGet("api/InviteCodes/paginated")]
[Authorize(Policy = AuthorizationPolicies.RequireAdminOrModeratorRole)]
public static partial class GetPaginatedInviteCodes
{
	public sealed record Query(int Page, int PerPage);

	private static async ValueTask<ReturnType> HandleAsync(
		Query request,
		ApplicationDbContext context,
		CancellationToken cancellationToken
	)
	{
		var codes = await context.InviteCodes
			.OrderByDescending(ic => ic.UsedDate)
			.ThenByDescending(ic => ic.IssueDate)
			.Paginate(request.Page, request.PerPage)
			.ProjectToDto()
			.ToArrayAsync(cancellationToken);

		return TypedResults.Ok(codes);
	}
}