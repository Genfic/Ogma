using Immediate.Apis.Shared;
using Immediate.Handlers.Shared;
using Immediate.Validations.Shared;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using Ogma3.Data;
using Ogma3.Data.Tags;

namespace Ogma3.Api.V1.Tags;

[Handler]
[MapGet("api/tags")]
public static partial class GetPaginatedTags
{
	[Validate]
	public sealed partial record Query(int Page, int PerPage) : IValidationTarget<Query>;

	private static async ValueTask<Ok<TagDto[]>> HandleAsync(
		Query request,
		ApplicationDbContext context,
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
}