using Immediate.Apis.Shared;
using Immediate.Handlers.Shared;
using Immediate.Validations.Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using Ogma3.Data;
using Ogma3.Data.Comments;
using Ogma3.Infrastructure.Extensions;
using Ogma3.Services.UserService;

namespace Ogma3.Api.V1.Comments;

using ReturnType = Results<UnauthorizedHttpResult, NotFound, Ok<long>>;

[Handler]
[MapDelete("api/comments/{commentId:long}")]
[Authorize]
public static partial class DeleteComment
{
	[Validate]
	public sealed partial record Command(long CommentId) : IValidationTarget<Command>;

	private static async ValueTask<ReturnType> HandleAsync(
		Command request,
		ApplicationDbContext context,
		IUserService userService,
		CancellationToken cancellationToken
	)
	{
		if (userService.User?.GetNumericId() is not {} uid) return TypedResults.Unauthorized();

		var rows = await context.Comments
			.Where(c => c.Id == request.CommentId)
			.Where(c => c.AuthorId == uid)
			.ExecuteUpdateAsync(setters => setters
					.SetProperty(c => c.DeletedBy, EDeletedBy.User)
					.SetProperty(c => c.DeletedByUserId, uid)
					.SetProperty(c => c.Body, string.Empty),
				cancellationToken);

		_ = await context.CommentRevisions
			.Where(r => r.ParentId == request.CommentId)
			.ExecuteDeleteAsync(cancellationToken);

		await context.SaveChangesAsync(cancellationToken);

		return rows > 0 ? TypedResults.Ok(request.CommentId) : TypedResults.NotFound();
	}
}