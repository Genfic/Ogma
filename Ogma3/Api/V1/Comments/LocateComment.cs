using Immediate.Apis.Shared;
using Immediate.Handlers.Shared;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using Ogma3.Data;
using Ogma3.Infrastructure.OgmaConfig;
using Sqids;

namespace Ogma3.Api.V1.Comments;

using ReturnType = Results<Ok<LocateComment.Response>, NotFound>;

[Handler]
[MapGet("api/comments/locate")]
[UsedImplicitly]
public sealed partial class LocateComment(ApplicationDbContext context, OgmaConfig config, SqidsEncoder<long> sqids)
{
	[UsedImplicitly]
	public sealed record Query(long ThreadId, string CommentId);

	private async ValueTask<ReturnType> Handle(Query request, CancellationToken cancellationToken)
	{
		if (sqids.Decode(request.CommentId) is not [var id])
		{
			return TypedResults.NotFound();
		}

		var exists = await context.Comments
			.Where(c => c.CommentsThreadId == request.ThreadId)
			.Where(c => c.Id == id)
			.AnyAsync(cancellationToken);

		if (!exists)
		{
			return TypedResults.NotFound();
		}

		var newer = await context.Comments
			.Where(c => c.CommentsThreadId == request.ThreadId)
			.Where(c => c.Id > id)
			.CountAsync(cancellationToken);

		var page = (newer / config.CommentsPerPage) + 1;

		return TypedResults.Ok(new Response(page));
	}

	public sealed record Response(int Page);
}