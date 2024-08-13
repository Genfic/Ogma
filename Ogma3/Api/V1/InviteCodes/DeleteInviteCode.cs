using Immediate.Apis.Shared;
using Immediate.Handlers.Shared;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using Ogma3.Data;
using Ogma3.Infrastructure.ServiceRegistrations;

namespace Ogma3.Api.V1.InviteCodes;

using ReturnType = Results<Ok<long>, NotFound>;

[Handler]
[MapDelete("api/InviteCodes/{CodeId:long}")]
[Authorize(Policy = AuthorizationPolicies.RequireAdminRole)]
public static partial class DeleteInviteCode
{
	[UsedImplicitly]
	public sealed record Command(long CodeId);

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