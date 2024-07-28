using Immediate.Apis.Shared;
using Immediate.Handlers.Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using Ogma3.Data;
using Ogma3.Infrastructure.Extensions;
using Ogma3.Services.UserService;

namespace Ogma3.Api.V1.Notifications;

using ReturnType = Results<Ok, NotFound, UnauthorizedHttpResult>;

[Handler]
[MapDelete("api/notifications/{id:long}")]
[Authorize]
public static partial class DeleteNotification
{
	public sealed record Command(long Id);
	
	private static async ValueTask<ReturnType> HandleAsync(
		Command request,
		ApplicationDbContext context,
		IUserService userService,
		CancellationToken cancellationToken
	)
	{
		if (userService.User?.GetNumericId() is not {} uid) return TypedResults.Unauthorized();

		var res = await context.NotificationRecipients
			.Where(nr => nr.RecipientId == uid)
			.Where(nr => nr.NotificationId == request.Id)
			.ExecuteDeleteAsync(cancellationToken);

		return res > 0 ? TypedResults.Ok() : TypedResults.NotFound();
	}

}