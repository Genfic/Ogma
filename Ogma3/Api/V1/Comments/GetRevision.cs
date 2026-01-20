using Immediate.Apis.Shared;
using Immediate.Handlers.Shared;
using Immediate.Validations.Shared;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using Ogma3.Data;
using Sqids;

namespace Ogma3.Api.V1.Comments;

[Handler]
[MapGet("api/comments/{commentId}/revisions")]
public static partial class GetRevision
{
	[Validate]
	[UsedImplicitly]
	public sealed partial record Query(string CommentId) : IValidationTarget<Query>;

	private static async ValueTask<Results<Ok<Result[]>, NotFound>> HandleAsync(
		Query request,
		ApplicationDbContext context,
		SqidsEncoder<long> sqids,
		CancellationToken cancellationToken
	)
	{
		if (sqids.Decode(request.CommentId) is not [var id])
		{
			return TypedResults.NotFound();
		}

		var revisions = await context.CommentRevisions
			.Where(r => r.ParentId == id)
			.Select(r => new Result(r.EditTime, r.Body))
			.ToArrayAsync(cancellationToken);

		return TypedResults.Ok(revisions);
	}

	public sealed record Result(DateTimeOffset EditTime, string Body);
}