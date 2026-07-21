using Immediate.Apis.Shared;
using Immediate.Handlers.Shared;
using Immediate.Validations.Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using Ogma3.Data;
using Ogma3.Infrastructure.ServiceRegistrations;
using Ogma3.Services.TagCache;

namespace Ogma3.Api.V1.Tags;

using ReturnType = Results<Ok<long>, NotFound>;

[Handler]
[MapGroup<ApiGroup>]
[MapDelete("tags")]
[Authorize(AuthorizationPolicies.RequireAdminRole)]
public sealed partial class DeleteTag(ApplicationDbContext context, TagCache cache)
{
	internal static void CustomizeEndpoint(RouteHandlerBuilder endpoint)
		=> endpoint
			.ProducesValidationProblem();

	private async ValueTask<ReturnType> HandleAsync(
		Command request,
		CancellationToken cancellationToken
	)
	{
		var tag = await context.Tags
			.Where(t => t.Id == request.TagId)
			.Select(t => new TagEntry(t.Id, t.Name, t.Namespace))
			.FirstOrDefaultAsync(cancellationToken);

		if (tag is null)
		{
			return TypedResults.NotFound();
		}

		var res = await context.Tags
			.Where(t => t.Id == request.TagId)
			.ExecuteDeleteAsync(cancellationToken);

		if (res <= 0)
		{
			return TypedResults.NotFound();
		}

		await cache.DeleteAsync(tag);
		return TypedResults.Ok(request.TagId);

	}

	[Validate]
	public sealed partial record Command(long TagId) : IValidationTarget<Command>;
}