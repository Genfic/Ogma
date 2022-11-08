using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Markdig;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Ogma3.Data;
using Ogma3.Data.Comments;
using Ogma3.Infrastructure.Constants;
using Ogma3.Infrastructure.Extensions;
using Ogma3.Infrastructure.MediatR.Bases;
using Ogma3.Services.UserService;

namespace Ogma3.Api.V1.Comments.Queries;

public static class GetComment
{
	public sealed record Query(long Id) : IRequest<ActionResult<CommentDto>>;

	public class Handler : BaseHandler, IRequestHandler<Query, ActionResult<CommentDto>>
	{
		private readonly ApplicationDbContext _context;
		private readonly long? _uid;

		public Handler(ApplicationDbContext context, IUserService userService)
		{
			_context = context;
			_uid = userService?.User?.GetNumericId();
		}

		public async Task<ActionResult<CommentDto>> Handle(Query request, CancellationToken cancellationToken)
		{
			var comment = await _context.Comments
				.Where(c => c.Id == request.Id)
				.Select(CommentMappings.ToCommentDto(_uid))
				.AsNoTracking()
				.FirstOrDefaultAsync(cancellationToken);

			comment.Body = Markdown.ToHtml(comment.Body, MarkdownPipelines.Comment);

			return comment;
		}
	}
}