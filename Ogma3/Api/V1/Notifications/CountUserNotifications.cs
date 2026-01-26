using Immediate.Apis.Shared;
using Immediate.Handlers.Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using Ogma3.Data;
using Ogma3.Services.UserService;

namespace Ogma3.Api.V1.Notifications;

using ReturnType = Results<Ok<int>, NoContent>;

[Handler]
[MapGet("api/notifications/count")]
[Authorize]
public static partial class CountUserNotifications
{
	public sealed record Query;

	private static async ValueTask<ReturnType> HandleAsync(
		Query _,
		ApplicationDbContext context,
		IUserService userService,
		CancellationToken cancellationToken
	)
	{
		if (userService.UserId is not {} uid) return TypedResults.NoContent();

		var count = await context.NotificationRecipients
			.Where(nr => nr.RecipientId == uid)
			.CountAsync(cancellationToken);

		return TypedResults.Ok(count);
	}
}