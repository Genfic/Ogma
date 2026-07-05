using Immediate.Apis.Shared;
using Immediate.Handlers.Shared;
using Immediate.Validations.Shared;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using Ogma3.Data;
using Ogma3.Data.Tags;

namespace Ogma3.Api.V1.Tags;

[Handler]
[MapGroup<ApiGroup>]
[MapGet("tags")]
public sealed partial class GetPaginatedTags(ApplicationDbContext context)
{
	internal static void CustomizeEndpoint(RouteHandlerBuilder endpoint)
		=> endpoint
			.ProducesValidationProblem();

	private async ValueTask<Ok<TagDto[]>> HandleAsync(
		Query request,
		CancellationToken cancellationToken
	)
	{
		var tags = await context.Tags
			.OrderBy(t => t.Id)
			.Paginate(request.Page, request.PerPage)
			.ProjectToDto()
			.ToArrayAsync(cancellationToken);

		return TypedResults.Ok(tags);
	}

	[Validate]
	public sealed partial record Query(int Page, int PerPage) : IValidationTarget<Query>;
}