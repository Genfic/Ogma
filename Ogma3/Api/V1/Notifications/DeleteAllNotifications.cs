using Immediate.Apis.Shared;
using Immediate.Handlers.Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using Ogma3.Data;
using Ogma3.Services.UserService;

namespace Ogma3.Api.V1.Notifications;

using ReturnType = Results<Ok, NotFound, UnauthorizedHttpResult>;

[Handler]
[MapDelete("api/notifications/all")]
[Authorize]
public sealed partial class DeleteAllNotifications(ApplicationDbContext context, IUserService userService)
{

	private async ValueTask<ReturnType> HandleAsync(
		Command _,
		CancellationToken cancellationToken
	)
	{
		if (userService.UserId is not {} uid) return TypedResults.Unauthorized();

		var res = await context.NotificationRecipients
			.Where(nr => nr.RecipientId == uid)
			.ExecuteDeleteAsync(cancellationToken);

		return res > 0 ? TypedResults.Ok() : TypedResults.NotFound();
	}

	public sealed record Command;
}