using Immediate.Apis.Shared;
using Immediate.Handlers.Shared;
using Immediate.Validations.Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using Ogma3.Data;
using Ogma3.Infrastructure.ServiceRegistrations;

namespace Ogma3.Api.V1.Tags;

using ReturnType = Results<Ok<long>, NotFound>;

[Handler]
[MapDelete("api/tags")]
[Authorize(AuthorizationPolicies.RequireAdminRole)]
public static partial class DeleteTag
{
	[Validate]
	public sealed partial record Command(long TagId) : IValidationTarget<Command>;

	private static async ValueTask<ReturnType> HandleAsync(
		Command request,
		ApplicationDbContext context,
		CancellationToken cancellationToken
	)
	{
		var res = await context.Tags
			.Where(t => t.Id == request.TagId)
			.ExecuteDeleteAsync(cancellationToken);

		return res > 0 ? TypedResults.Ok(request.TagId) : TypedResults.NotFound();
	}
}