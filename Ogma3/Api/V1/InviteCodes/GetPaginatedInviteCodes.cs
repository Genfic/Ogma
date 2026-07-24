using Immediate.Apis.Shared;
using Immediate.Handlers.Shared;
using Immediate.Validations.Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using Ogma3.Data;
using Ogma3.Data.InviteCodes;
using Ogma3.Infrastructure.Extensions;
using Ogma3.Infrastructure.ServiceRegistrations;

namespace Ogma3.Api.V1.InviteCodes;

using ReturnType = Ok<InviteCodeDto[]>;

[Handler]
[MapGroup<ApiGroup>]
[MapGet("InviteCodes/paginated")]
[Authorize(AuthorizationPolicies.RequireAdminOrModeratorRole)]
public sealed partial class GetPaginatedInviteCodes(ApplicationDbContext context)
{
	internal static void CustomizeEndpoint(RouteHandlerBuilder endpoint)
		=> endpoint
			.ProducesValidationProblem();

	private async ValueTask<ReturnType> HandleAsync(
		Query request,
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

	[Validate]
	public sealed partial record Query(int Page, int PerPage) : IValidationTarget<Query>;
}