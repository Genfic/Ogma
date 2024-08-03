using Mediator;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Ogma3.Data;
using Ogma3.Data.Tags;
using Ogma3.Infrastructure.Mediator.Bases;

namespace Ogma3.Api.V1.Tags.Queries;

public static class GetPaginatedTags
{
	public sealed record Query(int Page, int PerPage) : IRequest<ActionResult<List<TagDto>>>;

	public class Handler(ApplicationDbContext context) : BaseHandler, IRequestHandler<Query, ActionResult<List<TagDto>>>
	{
		public async ValueTask<ActionResult<List<TagDto>>> Handle(Query request, CancellationToken cancellationToken)
		{
			var (page, perPage) = request;

			var tags = await context.Tags
				.OrderBy(t => t.Id)
				.Paginate(page, perPage)
				.ProjectToDto()
				.ToListAsync(cancellationToken);

			return Ok(tags);
		}
	}
}