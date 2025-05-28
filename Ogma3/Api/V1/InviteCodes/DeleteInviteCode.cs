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
[MapDelete("api/InviteCodes/{CodeId:long}")]
[Authorize(AuthorizationPolicies.RequireAdminRole)]
public static partial class DeleteInviteCode
{
	[Validate]
	public sealed partial record Command(long CodeId) : IValidationTarget<Command>;

	private static async ValueTask<ReturnType> HandleAsync(
		Command request,
		ApplicationDbContext context,
		CancellationToken cancellationToken
	)
	{
		var res = await context.InviteCodes
			.Where(ic => ic.Id == request.CodeId)
			.ExecuteDeleteAsync(cancellationToken);

		return res > 0 ? TypedResults.Ok(request.CodeId) : TypedResults.NotFound();
	}
}