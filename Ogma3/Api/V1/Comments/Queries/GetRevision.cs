using Markdig;
using Mediator;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Ogma3.Data;
using Ogma3.Infrastructure.Constants;
using Ogma3.Infrastructure.Mediator.Bases;

namespace Ogma3.Api.V1.Comments.Queries;

public static class GetRevision
{
	public sealed record Query(long Id) : IRequest<ActionResult<IEnumerable<Result>>>;

	public class Handler(ApplicationDbContext context) : BaseHandler, IRequestHandler<Query, ActionResult<IEnumerable<Result>>>
	{

		public async ValueTask<ActionResult<IEnumerable<Result>>> Handle(Query request, CancellationToken cancellationToken)
			=> await context.CommentRevisions
				.Where(r => r.ParentId == request.Id)
				.Select(r => new Result(
					r.EditTime,
					Markdown.ToHtml(r.Body, MarkdownPipelines.Comment, null)
				))
				.ToArrayAsync(cancellationToken);
	}

	public sealed record Result(DateTime EditTime, string Body);
}