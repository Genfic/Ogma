using Mediator;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Ogma3.Data;
using Ogma3.Infrastructure.Mediator.Bases;

namespace Ogma3.Api.V1.Comments.Queries;

public static class GetCommentMarkdown
{
	public sealed record Query(long Id) : IRequest<ActionResult<string>>;

	public class Handler(ApplicationDbContext context) : BaseHandler, IRequestHandler<Query, ActionResult<string>>
	{

		public async ValueTask<ActionResult<string>> Handle(Query request, CancellationToken cancellationToken)
		{
			var markdown = await context.Comments
				.Where(c => c.Id == request.Id)
				.Select(c => c.Body)
				.FirstOrDefaultAsync(cancellationToken);
			
			return markdown is null ? NotFound() : Ok(markdown);
		}
	}
}