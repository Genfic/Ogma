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

using ReturnType = Results<UnauthorizedHttpResult, NotFound, Ok<string>>;

[Handler]
[MapGroup<ApiGroup>]
[MapDelete("comments/{commentId}")]
[Authorize]
public sealed partial class DeleteComment(AppDbContext context, IUserService userService, SqidsEncoder<long> sqids)
{
	internal static void CustomizeEndpoint(RouteHandlerBuilder endpoint)
		=> endpoint
			.ProducesValidationProblem();

	private async ValueTask<ReturnType> HandleAsync(
		Command request,
		CancellationToken cancellationToken
	)
	{
		if (sqids.Decode(request.CommentId) is not [var id])
		{
			return TypedResults.NotFound();
		}

		if (userService.UserId is not {} uid) return TypedResults.Unauthorized();

		// The transaction must be opened *before* the delete: ExecuteUpdate/ExecuteDelete outside a
		// transaction auto-commit individually, so a failure partway through would leave a comment
		// soft-deleted with its revisions still present, or a count decremented for a delete that
		// then failed.
		await using var transaction = await context.Database.BeginTransactionAsync(cancellationToken);

		var rows = await context.Comments
			.Where(c => c.Id == id)
			.Where(c => c.AuthorId == uid)
			.Where(c => c.DeletedBy == null)
			.ExecuteUpdateAsync(setPropertyCalls: setters => setters
					.SetProperty(propertyExpression: c => c.DeletedBy, EDeletedBy.User)
					.SetProperty(propertyExpression: c => c.DeletedByUserId, uid)
					.SetProperty(propertyExpression: c => c.Body, string.Empty),
				cancellationToken);

		if (rows == 0)
		{
			await transaction.RollbackAsync(cancellationToken);
			return TypedResults.NotFound();
		}

		_ = await context.CommentRevisions
			.Where(r => r.ParentId == id)
			.ExecuteDeleteAsync(cancellationToken);

		_ = await context.CommentThreads
				.Where(ct => ct.Comments.Any(c => c.Id == id))
				.ExecuteUpdateAsync(setters => setters
					.SetProperty(ct => ct.LastChange, DateTimeOffset.UtcNow)
					.SetProperty(ct => ct.CommentsCount, ct => ct.CommentsCount - 1),
				cancellationToken);

		await transaction.CommitAsync(cancellationToken);

		return TypedResults.Ok(request.CommentId);
	}

	[Validate]
	[UsedImplicitly]
	public sealed partial record Command(string CommentId) : IValidationTarget<Command>;
}