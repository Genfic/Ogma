using Immediate.Apis.Shared;
using Immediate.Handlers.Shared;
using Markdig;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using Ogma3.Data;
using Ogma3.Data.Comments;
using Ogma3.Infrastructure;
using Ogma3.Infrastructure.Constants;
using Ogma3.Infrastructure.Extensions;
using Ogma3.Services.UserService;

namespace Ogma3.Api.V1.Comments;

[Handler]
[MapGet("api/comments")]
public static partial class GetPaginatedComments
{
	public sealed record Query(long Thread, int? Page, long? Highlight);

	private static async ValueTask<Ok<PaginationResult<CommentDto>>> HandleAsync(
		Query request,
		ApplicationDbContext context,
		IUserService userService,
		OgmaConfig ogmaConfig,
		IHttpContextAccessor httpContextAccessor,
		CancellationToken cancellationToken
	)
	{
		var (thread, page, highlight) = request;

		var total = await context.CommentThreads
			.Where(ct => ct.Id == thread)
			.Select(ct => ct.CommentsCount)
			.FirstOrDefaultAsync(cancellationToken);

		// If a highlight has been requested, get the page on which the highlighted comment would be.
		// If not, simply return the requested page or the first page if requested page is null.
		// `highlight - 1` offsets the fact, that the requested IDs start from 1, not 0
		var p = highlight is not ({} h and > 0)
			? Math.Max(1, page ?? 1)
			: (int)Math.Ceiling((double)(total - (h - 1)) / ogmaConfig.CommentsPerPage);

		// Send auth data
		httpContextAccessor.HttpContext?.Response.Headers.Append("X-Authenticated",
			(userService.User?.Identity?.IsAuthenticated ?? false).ToString());

		var comments = await context.Comments
			.Where(c => c.CommentsThreadId == thread)
			.OrderByDescending(c => c.DateTime)
			.Select(CommentMappings.ToCommentDto(userService.User?.GetNumericId()))
			.Paginate(p, ogmaConfig.CommentsPerPage)
			.ToListAsync(cancellationToken);

		foreach (var comment in comments)
		{
			if (comment.Body is null) continue;
			comment.Body = Markdown.ToHtml(comment.Body, MarkdownPipelines.Comment);
		}

		var pagination = new PaginationResult<CommentDto>
		{
			Elements = comments,
			Total = total,
			Page = p,
			Pages = (int)Math.Ceiling((double)total / ogmaConfig.CommentsPerPage),
			PerPage = ogmaConfig.CommentsPerPage,
		};

		return TypedResults.Ok(pagination);
	}

}