using Mediator;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Ogma3.Data;
using Ogma3.Data.Tags;
using Ogma3.Infrastructure.Mediator.Bases;

namespace Ogma3.Api.V1.Tags.Queries;

public static class GetSingleTag
{
	public sealed record Query(long TagId) : IRequest<ActionResult<TagDto>>;

	public class Handler(ApplicationDbContext context) : BaseHandler, IRequestHandler<Query, ActionResult<TagDto>>
	{
		public async ValueTask<ActionResult<TagDto>> Handle(Query request, CancellationToken cancellationToken)
		{
			var tag = await context.Tags
				.Where(t => t.Id == request.TagId)
				.ProjectToDto()
				.FirstOrDefaultAsync(cancellationToken);

			return tag is null
				? NotFound()
				: Ok(tag);
		}
	}
}