using FluentValidation;
using Immediate.Apis.Shared;
using Immediate.Handlers.Shared;
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
	public sealed record Command(string Body, long CommentId);

	public sealed class CommandValidator : AbstractValidator<Command>
	{
		public CommandValidator()
			=> RuleFor(c => c.Body)
				.MinimumLength(CTConfig.Comment.MinBodyLength)
				.MaximumLength(CTConfig.Comment.MaxBodyLength);
	}

	private static async ValueTask<ReturnType> HandleAsync(
		Command request,
		ApplicationDbContext context,
		IUserService userService,
		CancellationToken cancellationToken
	)
	{
		if (userService.User?.GetNumericId() is not {} uid) return TypedResults.Unauthorized();

		var (body, commentId) = request;

		var comment = await context.Comments
			.Where(c => c.Id == commentId)
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
		comment.Body = body;

		await context.SaveChangesAsync(cancellationToken);

		return TypedResults.Ok(new Response(comment.Body, DateTimeOffset.UtcNow));
	}

	public sealed record Response(string Body, DateTimeOffset EditTime);
}