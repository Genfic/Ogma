using Immediate.Apis.Shared;
using Immediate.Handlers.Shared;
using Immediate.Validations.Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using Ogma3.Data;
using Ogma3.Infrastructure.ServiceRegistrations;

namespace Ogma3.Api.V1.InviteCodes;

using ReturnType = Results<Ok<long>, NotFound>;

[Handler]
[MapGroup<ApiGroup>]
[MapDelete("InviteCodes/{CodeId:long}")]
[Authorize(AuthorizationPolicies.RequireAdminRole)]
public sealed partial class DeleteInviteCode(ApplicationDbContext context)
{
	internal static void CustomizeEndpoint(RouteHandlerBuilder endpoint)
		=> endpoint
			.ProducesValidationProblem();

	private async ValueTask<ReturnType> HandleAsync(
		Command request,
		CancellationToken cancellationToken
	)
	{
		var res = await context.InviteCodes
			.Where(ic => ic.Id == request.CodeId)
			.ExecuteDeleteAsync(cancellationToken);

		return res > 0 ? TypedResults.Ok(request.CodeId) : TypedResults.NotFound();
	}

	[Validate]
	public sealed partial record Command(long CodeId) : IValidationTarget<Command>;
}