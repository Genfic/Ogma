using Immediate.Apis.Shared;
using Immediate.Handlers.Shared;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using Ogma3.Data;

namespace Ogma3.Api.V1.Comments;

[Handler]
[MapGet("api/comments/{commentId:long}/revisions")]
public static partial class GetRevision
{
	public sealed record Query(long CommentId);

	private static async ValueTask<Ok<Result[]>> HandleAsync(
		Query request,
		ApplicationDbContext context,
		CancellationToken cancellationToken
	)
	{
		var revisions = await context.CommentRevisions
			.Where(r => r.ParentId == request.CommentId)
			.Select(r => new Result(r.EditTime, r.Body))
			.ToArrayAsync(cancellationToken);

		return TypedResults.Ok(revisions);
	}
	
	public sealed record Result(DateTime EditTime, string Body);
}