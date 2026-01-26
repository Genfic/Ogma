using Immediate.Apis.Shared;
using Immediate.Handlers.Shared;
using Immediate.Validations.Shared;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using Ogma3.Data;
using Ogma3.Data.Comments;
using Ogma3.Services.UserService;
using Sqids;

namespace Ogma3.Api.V1.Comments;

using ReturnType = Results<UnauthorizedHttpResult, NotFound, Ok<UpdateComment.Response>>;

[Handler]
[MapPatch("api/comments")]
[Authorize]
public static partial class UpdateComment
{

	[Validate]
	[UsedImplicitly]
	public sealed partial record Command : IValidationTarget<Command>
	{
		[MaxLength(CTConfig.Comment.MaxBodyLength)]
		[MinLength(CTConfig.Comment.MinBodyLength)]
		public required string Body { get; init; }
		public required string CommentId { get; init; }

	}

	private static async ValueTask<ReturnType> HandleAsync(
		Command request,
		ApplicationDbContext context,
		IUserService userService,
		SqidsEncoder<long> sqids,
		CancellationToken cancellationToken
	)
	{
		if (sqids.Decode(request.CommentId) is not [var id])
		{
			return TypedResults.NotFound();
		}

		if (userService.UserId is not {} uid) return TypedResults.Unauthorized();

		var comment = await context.Comments
			.Where(c => c.Id == id)
			.Where(c => c.AuthorId == uid)
			.FirstOrDefaultAsync(cancellationToken);

		if (comment is null) return TypedResults.NotFound();

		context.CommentRevisions.Add(new CommentRevision
		{
			Body = comment.Body,
			ParentId = comment.Id,
		});

		comment.Body = request.Body;

		await context.SaveChangesAsync(cancellationToken);

		return TypedResults.Ok(new Response(comment.Body, DateTimeOffset.UtcNow));
	}

	public sealed record Response(string Body, DateTimeOffset EditTime);
}