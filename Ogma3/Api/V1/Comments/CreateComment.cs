using Immediate.Apis.Shared;
using Immediate.Handlers.Shared;
using Immediate.Validations.Shared;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using Ogma3.Data;
using Ogma3.Data.Comments;
using Ogma3.Data.Infractions;
using Ogma3.Services.PowService;
using Ogma3.Services.UserService;
using Sqids;

namespace Ogma3.Api.V1.Comments;

using ReturnType = Results<UnauthorizedHttpResult, BadRequest<string>, NotFound, Ok<string>>;

[Handler]
[MapGroup<ApiGroup>]
[MapPost("comments")]
[Authorize]
[UsedImplicitly]
public sealed partial class CreateComment
(
	ApplicationDbContext context,
	IUserService userService,
	PowService powService,
	SqidsEncoder<long> sqids)
{

	private async ValueTask<ReturnType> HandleAsync(
		Command request,
		CancellationToken cancellationToken
	)
	{
		if (userService.UserId is not {} uid) return TypedResults.Unauthorized();

		var powResult = await powService.VerifyChallenge(request.PowToken, request.PowNonce, request.PowHash);

		if (powResult is not PowVerificationResult.Ok)
		{
			return TypedResults.BadRequest($"Invalid challenge: {powResult.ToStringFast()}");
		}

		var isMuted = await CheckIfMuted(context, uid, cancellationToken);
		if (isMuted)
		{
			return TypedResults.Unauthorized();
		}

		var thread = await context.CommentThreads
			.Where(ct => ct.Id == request.Thread)
			.FirstOrDefaultAsync(cancellationToken);

		if (thread is null) return TypedResults.NotFound();
		if (thread.LockDate is not null)
		{
			return TypedResults.Unauthorized();
		}

		var isBlocked = await CheckIfBlocked(context, thread.UserId, uid, request.Source, cancellationToken);
		if (isBlocked)
		{
			return TypedResults.Unauthorized();
		}

		var comment = new Comment
		{
			AuthorId = uid,
			Body = request.Body,
			CommentsThreadId = request.Thread,
		};

		context.Comments.Add(comment);
		thread.CommentsCount++;
		thread.LastChange = DateTimeOffset.UtcNow;

		await context.SaveChangesAsync(cancellationToken);

		// TODO: Make the notification shit better
		// await notificationsRepo.NotifyUsers(thread.Id, comment.Id, comment.Body.Truncate(50), cancellationToken, [uid]);

		return TypedResults.Ok(sqids.Encode(comment.Id));
	}

	private static async ValueTask<bool> CheckIfMuted(ApplicationDbContext context, long currentUserId, CancellationToken ct)
	{
		return await context.Infractions
			.Where(i => i.UserId == currentUserId)
			.Where(i => i.Type == InfractionType.Mute)
			.Where(i => i.RemovedAt == null)
			.Where(i => i.ActiveUntil > DateTimeOffset.UtcNow)
			.AnyAsync(ct);
	}

	/// <summary>
	///     Check if the comment thread is a profile thread, and if so, if the profile owner has the current user blocked
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

	[Validate]
	[UsedImplicitly]
	public sealed partial record Command : IValidationTarget<Command>
	{
		[MaxLength(CTConfig.Comment.MaxBodyLength)]
		[MinLength(CTConfig.Comment.MinBodyLength)]
		public required string Body { get; init; }
		public required long Thread { get; init; }
		public required CommentSource Source { get; init; }
		public required string PowToken { get; init; }
		public required int PowNonce { get; init; }
		public required string PowHash { get; init; }
	}
}