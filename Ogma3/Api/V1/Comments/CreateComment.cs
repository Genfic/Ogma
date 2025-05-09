using FluentValidation;
using Immediate.Apis.Shared;
using Immediate.Handlers.Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using Ogma3.Data;
using Ogma3.Data.Comments;
using Ogma3.Data.Infractions;
using Ogma3.Infrastructure.Extensions;
using Ogma3.Services.UserService;

namespace Ogma3.Api.V1.Comments;

using ReturnType = Results<UnauthorizedHttpResult, NotFound, CreatedAtRoute>;

[Handler]
[MapPost("api/comments")]
[Authorize]
public static partial class CreateComment
{
	public sealed record Command(string Body, long Thread, CommentSource Source);

	public sealed class CommandValidator : AbstractValidator<Command>
	{
		public CommandValidator() => RuleFor(c => c.Body)
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

		var isMuted = await CheckIfMuted(context, uid, cancellationToken);
		if (isMuted) return TypedResults.Unauthorized();

		var thread = await context.CommentThreads
			.Where(ct => ct.Id == request.Thread)
			.FirstOrDefaultAsync(cancellationToken);

		if (thread is null) return TypedResults.NotFound();
		if (thread.LockDate is not null) return TypedResults.Unauthorized();

		var isBlocked = await CheckIfBlocked(context, thread.UserId, uid, request.Source, cancellationToken);
		if (isBlocked) return TypedResults.Unauthorized();

		var comment = new Comment
		{
			AuthorId = uid,
			Body = request.Body,
			CommentsThreadId = request.Thread,
		};

		context.Comments.Add(comment);
		thread.CommentsCount++;

		await context.SaveChangesAsync(cancellationToken);
		
		// TODO: Make the notification shit better
		// await notificationsRepo.NotifyUsers(thread.Id, comment.Id, comment.Body.Truncate(50), cancellationToken, [uid]);

		return TypedResults.CreatedAtRoute(nameof(GetComment), new GetComment.Query(comment.Id));
	}

	private static async ValueTask<bool> CheckIfMuted(ApplicationDbContext context, long currentUserId, CancellationToken ct)
	{
		return await context.Infractions
			.Where(i => i.UserId == currentUserId)
			.Where(i => i.Type == InfractionType.Mute)
			.AnyAsync(ct);
	}

	/// <summary>
	/// Check if comment thread is a profile thread, and if so, if the profile owner has the current user blocked
	/// </summary>
	private static async ValueTask<bool> CheckIfBlocked(
		ApplicationDbContext context,
		long? profileOwnerId,
		long currentUserId,
		CommentSource source,
		CancellationToken ct
	)
	{
		if (source != CommentSource.Profile) return false;
		
		return await context.BlockedUsers
			.Where(b => b.BlockingUserId == profileOwnerId)
			.Where(b => b.BlockedUserId == currentUserId)
			.AnyAsync(ct);
	}
}