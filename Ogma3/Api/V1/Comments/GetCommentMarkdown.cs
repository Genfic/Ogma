using Immediate.Apis.Shared;
using Immediate.Handlers.Shared;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using Ogma3.Data;

namespace Ogma3.Api.V1.Comments;

using ReturnType = Results<Ok<string>, NotFound>;

[Handler]
[MapGet("api/comments/{commentId:int}/md")]
public static partial class GetCommentMarkdown
{
	public sealed record Query(long CommentId);

	private static async ValueTask<ReturnType> HandleAsync(Query request, ApplicationDbContext context, CancellationToken cancellationToken)
	{
		var markdown = await context.Comments
			.Where(c => c.Id == request.CommentId)
			.Select(c => c.Body)
			.FirstOrDefaultAsync(cancellationToken);

		return markdown is null ? TypedResults.NotFound() : TypedResults.Ok(markdown);
	}
}