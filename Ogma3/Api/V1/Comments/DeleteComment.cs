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
public sealed partial class DeleteComment(ApplicationDbContext context, IUserService userService, SqidsEncoder<long> sqids)
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

		var rows = await context.Comments
			.Where(c => c.Id == id)
			.Where(c => c.AuthorId == uid)
			.ExecuteUpdateAsync(setPropertyCalls: setters => setters
					.SetProperty(propertyExpression: c => c.DeletedBy, EDeletedBy.User)
					.SetProperty(propertyExpression: c => c.DeletedByUserId, uid)
					.SetProperty(propertyExpression: c => c.Body, string.Empty),
				cancellationToken);

		_ = await context.CommentRevisions
			.Where(r => r.ParentId == id)
			.ExecuteDeleteAsync(cancellationToken);

		_ = await context.CommentThreads
				.Where(ct => ct.Comments.Any(c => c.Id == id))
				.ExecuteUpdateAsync(setters => setters
					.SetProperty(ct => ct.LastChange, DateTimeOffset.UtcNow)
					.SetProperty(ct => ct.CommentsCount, ct => ct.CommentsCount - 1),
				cancellationToken);

		await context.SaveChangesAsync(cancellationToken);

		return rows > 0 ? TypedResults.Ok(request.CommentId) : TypedResults.NotFound();
	}

	[Validate]
	[UsedImplicitly]
	public sealed partial record Command(string CommentId) : IValidationTarget<Command>;
}