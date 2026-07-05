using Immediate.Apis.Shared;
using Immediate.Handlers.Shared;
using Immediate.Validations.Shared;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using Ogma3.Data;
using Ogma3.Data.Tags;

namespace Ogma3.Api.V1.Tags;

using ReturnType = Results<Ok<TagDto[]>, NotFound>;

[Handler]
[MapGroup<ApiGroup>]
[MapGet("tags/story/{storyId:long}")]
public sealed partial class GetStoryTags(ApplicationDbContext context)
{
	internal static void CustomizeEndpoint(RouteHandlerBuilder endpoint)
		=> endpoint
			.ProducesValidationProblem();

	private async ValueTask<ReturnType> HandleAsync(Query request, CancellationToken cancellationToken)
	{
		var tags = await context.StoryTags
			.Where(st => st.StoryId == request.StoryId)
			.Select(st => st.Tag)
			.ProjectToDto()
			.ToArrayAsync(cancellationToken);

		return tags is { Length: > 0 }
			? TypedResults.Ok(tags)
			: TypedResults.NotFound(); // NOTE: Maybe this should be *only* returned when story is not found?
	}

	[Validate]
	public sealed partial record Query(long StoryId) : IValidationTarget<Query>;
}