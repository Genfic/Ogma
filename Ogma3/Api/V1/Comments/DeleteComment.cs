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
[MapDelete("api/comments/{commentId}")]
[Authorize]
public static partial class DeleteComment
{
	[Validate]
	[UsedImplicitly]
	public sealed partial record Command(string CommentId) : IValidationTarget<Command>;

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

		var rows = await context.Comments
			.Where(c => c.Id == id)
			.Where(c => c.AuthorId == uid)
			.ExecuteUpdateAsync(setters => setters
					.SetProperty(c => c.DeletedBy, EDeletedBy.User)
					.SetProperty(c => c.DeletedByUserId, uid)
					.SetProperty(c => c.Body, string.Empty),
				cancellationToken);

		_ = await context.CommentRevisions
			.Where(r => r.ParentId == id)
			.ExecuteDeleteAsync(cancellationToken);

		await context.SaveChangesAsync(cancellationToken);

		return rows > 0 ? TypedResults.Ok(request.CommentId) : TypedResults.NotFound();
	}
}