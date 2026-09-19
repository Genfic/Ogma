using System.Globalization;
using System.Security.Claims;
using Humanizer;
using Immediate.Injections.Shared;
using Ogma3.Data;
using Ogma3.Data.Infractions;
using Ogma3.Data.ModeratorActions;
using Ogma3.Infrastructure.Exceptions;
using Ogma3.Infrastructure.Extensions;

namespace Ogma3.Services.ModeratorActionService;

[RegisterScoped<IModeratorActionService>]
public sealed class ModeratorActionService
(
	AppDbContext context,
	IHttpContextAccessor httpContextAccessor
) : IModeratorActionService
{
	private static NotAuthenticatedException Unauthenticated() => new("Cannot log moderator action: user not authenticated");

	private ClaimsPrincipal User => httpContextAccessor.HttpContext?.User ?? throw Unauthenticated();
	private long UserId => User.GetNumericId() ?? throw Unauthenticated();
	private string UserName => User.GetUsername() ?? throw Unauthenticated();

	private static string? HumanizeTimespan(TimeSpan? ts)
		=> ts?.Humanize(2, minUnit: TimeUnit.Minute, culture: CultureInfo.InvariantCulture);

	private ModeratorAction CreateAction(string description)
	{
		return new ModeratorAction
		{
			StaffMemberId = UserId,
			Description = description,
		};
	}

	public void LogInfractionCreated(
		long userId,
		long infractionId,
		string reason,
		InfractionType type
	)
	{
		var action = CreateAction(
			$"""User **{userId}** was given a **{type.ToStringFast()}** infraction ({infractionId}) by **{UserName}** for the following reason: "*{reason}*" """);
		context.ModeratorActions.Add(action);
	}

	public void LogInfractionLifted(
		long userId,
		long infractionId,
		DateTimeOffset? activeUntil,
		DateTimeOffset? removedAt,
		InfractionType type
	)
	{
		var early = HumanizeTimespan(activeUntil - removedAt) ?? "[unknown]";
		var action = CreateAction($"User **{userId}** had their **{type.ToStringFast()}** infraction ({infractionId}) lifted by **{UserName}** {early} early.");
		context.ModeratorActions.Add(action);
	}

	public void LogUserRolesChanged(
		long targetUserId,
		string targetUserName,
		long[] oldRoles,
		long[] newRoles
	)
	{
		var action = CreateAction(
			$"User **{targetUserName}** (id: {targetUserId}) had their roles changed by **{UserName}** from [{string.Join(", ", oldRoles)}] to [{string.Join(", ", newRoles)}].");
		context.ModeratorActions.Add(action);
	}

	public void LogContentBlocked(
		string contentType,
		string title,
		long contentId
	)
	{
		var action = CreateAction($"""{contentType.Humanize()} ***"{title}"*** (id: {contentId}) has been blocked by **{UserName}**""");
		context.ModeratorActions.Add(action);
	}

	public void LogContentUnblocked(
		string contentType,
		string title,
		long contentId
	)
	{
		var action = CreateAction($"""{contentType.Humanize()} ***"{title}"*** (id: {contentId}) has been unblocked by **{UserName}**""");
		context.ModeratorActions.Add(action);
	}

	public void LogThreadLocked(
		string contentType,
		long contentId,
		long threadId
	)
	{
		var action = CreateAction(
			$"Comment thread for **{contentType}** (id: {contentId}) with the ID **{threadId}** was locked by **{UserName}**");
		context.ModeratorActions.Add(action);
	}

	public void LogThreadUnlocked(
		string contentType,
		long contentId,
		long threadId
	)
	{
		var action = CreateAction(
			$"Comment thread for **{contentType}** (id: {contentId}) with the ID **{threadId}** was unlocked by **{UserName}**");
		context.ModeratorActions.Add(action);
	}
}