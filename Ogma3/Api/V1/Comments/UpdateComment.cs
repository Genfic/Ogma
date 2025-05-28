using Immediate.Apis.Shared;
using Immediate.Handlers.Shared;
using Immediate.Validations.Shared;
using Markdig;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using Ogma3.Data;
using Ogma3.Data.Comments;
using Ogma3.Infrastructure.Constants;
using Ogma3.Infrastructure.Extensions;
using Ogma3.Services.UserService;

namespace Ogma3.Api.V1.Comments;

using ReturnType = Results<UnauthorizedHttpResult, NotFound, Ok<UpdateComment.Response>>;

[Handler]
[MapPatch("api/comments")]
[Authorize]
public static partial class UpdateComment
{

	[Validate]
	public sealed partial record Command : IValidationTarget<Command>
	{
		[MaxLength(CTConfig.Comment.MaxBodyLength)]
		[MinLength(CTConfig.Comment.MinBodyLength)]
		public required string Body { get; init; }
		public required long CommentId { get; init; }

	}

	private static async ValueTask<ReturnType> HandleAsync(
		Command request,
		ApplicationDbContext context,
		IUserService userService,
		CancellationToken cancellationToken
	)
	{
		if (userService.User?.GetNumericId() is not {} uid) return TypedResults.Unauthorized();

		var comment = await context.Comments
			.Where(c => c.Id == request.CommentId)
			.Where(c => c.AuthorId == uid)
			.FirstOrDefaultAsync(cancellationToken);

		if (comment is null) return TypedResults.NotFound();

		// Create revision
		context.CommentRevisions.Add(new CommentRevision
		{
			Body = Markdown.ToHtml(comment.Body, MarkdownPipelines.Comment),
			ParentId = comment.Id,
		});

		// Edit the comment
		comment.Body = request.Body;

		await context.SaveChangesAsync(cancellationToken);

		return TypedResults.Ok(new Response(comment.Body, DateTimeOffset.UtcNow));
	}

	public sealed record Response(string Body, DateTimeOffset EditTime);
}