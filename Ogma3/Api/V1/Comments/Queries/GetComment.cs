using Markdig;
using Mediator;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Ogma3.Data;
using Ogma3.Data.Comments;
using Ogma3.Infrastructure.Constants;
using Ogma3.Infrastructure.Extensions;
using Ogma3.Infrastructure.Mediator.Bases;
using Ogma3.Services.UserService;

namespace Ogma3.Api.V1.Comments.Queries;

public static class GetComment
{
	public sealed record Query(long Id) : IRequest<ActionResult<CommentDto>>;

	public class Handler
		(ApplicationDbContext context, IUserService userService) : BaseHandler, IRequestHandler<Query, ActionResult<CommentDto>>
	{
		private readonly long? _uid = userService.User?.GetNumericId();

		public async ValueTask<ActionResult<CommentDto>> Handle(Query request, CancellationToken cancellationToken)
		{
			var comment = await context.Comments
				.Where(c => c.Id == request.Id)
				.Select(CommentMappings.ToCommentDto(_uid))
				.AsNoTracking()
				.FirstOrDefaultAsync(cancellationToken);

			if (comment is null) return NotFound();
			
			comment.Body = Markdown.ToHtml(comment.Body, MarkdownPipelines.Comment);

			return Ok(comment);
		}
	}
}