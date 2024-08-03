using Mediator;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Ogma3.Data;
using Ogma3.Data.Tags;
using Ogma3.Infrastructure.Mediator.Bases;

namespace Ogma3.Api.V1.Tags.Queries;

public static class GetStoryTags
{
	public sealed record Query(long StoryId) : IRequest<ActionResult<List<TagDto>>>;

	public class Handler(ApplicationDbContext context) : BaseHandler, IRequestHandler<Query, ActionResult<List<TagDto>>>
	{

		public async ValueTask<ActionResult<List<TagDto>>> Handle(Query request, CancellationToken cancellationToken)
		{
			var tags = await context.StoryTags
				.Where(st => st.StoryId == request.StoryId)
				.Select(st => st.Tag)
				.ProjectToDto()
				.AsNoTracking()
				.ToListAsync(cancellationToken);

			return tags is { Count: > 0 }
				? Ok(tags)
				: NotFound();
		}
	}
}