using Immediate.Apis.Shared;
using Immediate.Handlers.Shared;
using Immediate.Validations.Shared;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using Ogma3.Data;
using Ogma3.Data.Comments;
using Ogma3.Infrastructure.Constants;
using Ogma3.Infrastructure.Extensions;
using Ogma3.Infrastructure.ServiceRegistrations;
using Ogma3.Services.ModeratorActionService;
using Ogma3.Services.UserService;

namespace Ogma3.Api.V1.CommentThreads;

using ReturnType = Results<UnauthorizedHttpResult, NotFound, Ok<bool>>;

[Handler]
[MapGroup<ApiGroup>]
[MapPost("CommentsThread/lock")]
[Authorize(AuthorizationPolicies.RequireAdminOrModeratorRole)]
public sealed partial class LockThread(AppDbContext context, IUserService userService, IModeratorActionService moderatorActionService)
{
	internal static void CustomizeEndpoint(RouteHandlerBuilder endpoint)
		=> endpoint
			.ProducesValidationProblem();

	private async ValueTask<ReturnType> HandleAsync(
		Command request,
		CancellationToken cancellationToken
	)
	{
		if (userService.User is not {} user) return TypedResults.Unauthorized();
		if (!user.HasAnyRole(RoleNames.Admin, RoleNames.Moderator)) return TypedResults.Unauthorized();

		var thread = await context.CommentThreads
			.Where(ct => ct.Id == request.ThreadId)
			.FirstOrDefaultAsync(cancellationToken);

		if (thread is null)
		{
			return TypedResults.NotFound();
		}

		thread.LockDate = thread.LockDate is null ? DateTimeOffset.UtcNow : null;

		if (thread.IsLocked)
		{
			moderatorActionService.LogThreadLocked(thread.Source.ToStringFast(), thread.SourceId, thread.Id);
		}
		else
		{
			moderatorActionService.LogThreadUnlocked(thread.Source.ToStringFast(), thread.SourceId, thread.Id);
		}

		await context.SaveChangesAsync(cancellationToken);

		return TypedResults.Ok(thread.LockDate is not null);
	}

	[Validate]
	[UsedImplicitly]
	public sealed partial record Command(long ThreadId) : IValidationTarget<Command>;
}