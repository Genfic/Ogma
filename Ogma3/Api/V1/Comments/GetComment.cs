using Immediate.Apis.Shared;
using Immediate.Handlers.Shared;
using Markdig;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using Ogma3.Data;
using Ogma3.Data.Comments;
using Ogma3.Infrastructure.Constants;
using Ogma3.Infrastructure.Extensions;
using Ogma3.Services.UserService;

namespace Ogma3.Api.V1.Comments;

using ReturnType = Results<UnauthorizedHttpResult, NotFound, Ok<CommentDto>>;

[Handler]
[MapGet("api/comments/{commentId:long}")]
public static partial class GetComment
{
	internal static void CustomizeEndpoint(IEndpointConventionBuilder endpoint) => endpoint.WithName(nameof(GetComment));

	public sealed record Query(long CommentId);

	private static async ValueTask<ReturnType> HandleAsync(
		Query request,
		ApplicationDbContext context,
		IUserService userService,
		CancellationToken cancellationToken
	)
	{
		if (userService.User?.GetNumericId() is not {} uid) return TypedResults.Unauthorized();

		var comment = await context.Comments
			.Where(c => c.Id == request.CommentId)
			.Select(CommentMappings.ToCommentDto(uid))
			.FirstOrDefaultAsync(cancellationToken);

		if (comment is null) return TypedResults.NotFound();

		if (comment.Body is not null)
		{
			comment.Body = Markdown.ToHtml(comment.Body, MarkdownPipelines.Comment);
		}

		return TypedResults.Ok(comment);
	}
}