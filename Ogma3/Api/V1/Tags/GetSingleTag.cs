using Immediate.Apis.Shared;
using Immediate.Handlers.Shared;
using Immediate.Validations.Shared;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using Ogma3.Data;
using Ogma3.Data.Tags;

namespace Ogma3.Api.V1.Tags;

using ReturnType = Results<Ok<TagDto>, NotFound>;

[Handler]
[MapGet("api/tags/{tagId:long}")]
public static partial class GetSingleTag
{
	internal static void CustomizeEndpoint(IEndpointConventionBuilder endpoint) => endpoint.WithName(nameof(GetSingleTag));

	[Validate]
	public sealed partial record Query(long TagId) : IValidationTarget<Query>;

	private static async ValueTask<ReturnType> HandleAsync(
		Query request,
		ApplicationDbContext context,
		CancellationToken cancellationToken
	)
	{
		var tag = await context.Tags
			.Where(t => t.Id == request.TagId)
			.ProjectToDto()
			.FirstOrDefaultAsync(cancellationToken);

		return tag is null
			? TypedResults.NotFound()
			: TypedResults.Ok(tag);
	}
}