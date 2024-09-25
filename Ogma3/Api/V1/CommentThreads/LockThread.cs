using Immediate.Apis.Shared;
using Immediate.Handlers.Shared;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using Ogma3.Data;
using Ogma3.Data.ModeratorActions;
using Ogma3.Infrastructure.Constants;
using Ogma3.Infrastructure.Extensions;
using Ogma3.Infrastructure.ServiceRegistrations;
using Ogma3.Services.UserService;

namespace Ogma3.Api.V1.CommentThreads;

using ReturnType = Results<UnauthorizedHttpResult, NotFound, Ok<bool>>;

[Handler]
[MapPost("api/CommentsThread/lock")]
[Authorize(AuthorizationPolicies.RequireAdminOrModeratorRole)]
public static partial class LockThread
{
	[UsedImplicitly]
	public sealed record Command(long ThreadId);

	private static async ValueTask<ReturnType> HandleAsync(
		Command request,
		ApplicationDbContext context,
		IUserService userService,
		CancellationToken cancellationToken
	)
	{
		if (userService.User is not {} user) return TypedResults.Unauthorized();
		if (user.GetUsername() is not {} uname) return TypedResults.Unauthorized();
		if (user.GetNumericId() is not {} uid) return TypedResults.Unauthorized();
		if (!user.HasAnyRole(RoleNames.Admin, RoleNames.Moderator)) return TypedResults.Unauthorized();

		var thread = await context.CommentThreads
			.Where(ct => ct.Id == request.ThreadId)
			.FirstOrDefaultAsync(cancellationToken);

		if (thread is null) return TypedResults.NotFound();

		thread.LockDate = thread.LockDate is null ? DateTimeOffset.UtcNow : null;

		string type;
		if (thread.BlogpostId is not null) type = "blogpost";
		else if (thread.ChapterId is not null) type = "chapter";
		else if (thread.ClubThreadId is not null) type = "club";
		else if (thread.UserId is not null) type = "user profile";
		else type = "unknown";

		var typeId = thread.BlogpostId ?? thread.ChapterId ?? thread.ClubThreadId ?? thread.UserId ?? 0;

		var message = thread.IsLocked
			? ModeratorActionTemplates.ThreadLocked(type, typeId, thread.Id, uname)
			: ModeratorActionTemplates.ThreadUnlocked(type, typeId, thread.Id, uname);

		context.ModeratorActions.Add(new ModeratorAction
		{
			StaffMemberId = uid,
			Description = message,
		});

		await context.SaveChangesAsync(cancellationToken);

		return TypedResults.Ok(thread.LockDate is not null);
	}
}