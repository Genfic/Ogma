using System.Collections.Concurrent;
using System.Globalization;
using System.Security.Claims;
using Humanizer;
using Immediate.Injections.Shared;
using Microsoft.EntityFrameworkCore;
using Ogma3.Data;
using Ogma3.Data.Infractions;
using Ogma3.Data.ModeratorActions;
using Ogma3.Infrastructure.Exceptions;
using Ogma3.Infrastructure.Extensions;

namespace Ogma3.Services.ModeratorActionService;

// *Must* be scoped to be disposed properly so the logs can save in bulk
[RegisterScoped<IModeratorActionService>]
public sealed class ModeratorActionService
(
	IDbContextFactory<AppDbContext> contextFactory,
	IHttpContextAccessor httpContextAccessor,
	ILogger<ModeratorActionService> logger
) : IModeratorActionService, IAsyncDisposable
{
	private static NotAuthenticatedException Unauthenticated() => new("Cannot log moderator action: user not authenticated");

	private ClaimsPrincipal User => httpContextAccessor.HttpContext?.User ?? throw Unauthenticated();
	private long UserId => User.GetNumericId() ?? throw Unauthenticated();
	private string UserName => User.GetUsername() ?? throw Unauthenticated();

	private ConcurrentBag<string> LogQueue { get; } = [];

	private static string? HumanizeTimespan(TimeSpan? ts)
		=> ts?.Humanize(2, minUnit: TimeUnit.Minute, culture: CultureInfo.InvariantCulture);

	public async ValueTask DisposeAsync()
	{
		if (LogQueue.IsEmpty)
		{
			return;
		}

		var context = await contextFactory.CreateDbContextAsync();

		context.ModeratorActions.AddRange(LogQueue.Select(description => new ModeratorAction
		{
			StaffMemberId = UserId,
			Description = description,
		}));

		var count = await context.SaveChangesAsync();

		logger.LogInformation("Dumped {Rows}/{Count} moderator actions to database", count, LogQueue.Count);
		LogQueue.Clear();
	}

	public void LogInfractionCreated(
		long userId,
		long infractionId,
		string reason,
		InfractionType type
	) => LogQueue.Add($"""User **{userId}** was given a **{type.ToStringFast()}** infraction ({infractionId}) by **{UserName}** for the following reason: "*{reason}*" """);

	public void LogInfractionLifted(
		long userId,
		long infractionId,
		DateTimeOffset? activeUntil,
		DateTimeOffset? removedAt,
		InfractionType type
	)
	{
		var early = HumanizeTimespan(activeUntil - removedAt) ?? "[unknown]";
		LogQueue.Add($"User **{userId}** had their **{type.ToStringFast()}** infraction ({infractionId}) lifted by **{UserName}** {early} early.");
	}

	public void LogUserRolesChanged(
		long targetUserId,
		string targetUserName,
		long[] oldRoles,
		long[] newRoles
	) => LogQueue.Add($"User **{targetUserName}** (id: {targetUserId}) had their roles changed by **{UserName}** from [{string.Join(", ", oldRoles)}] to [{string.Join(", ", newRoles)}].");

	public void LogContentBlocked(
		string contentType,
		string title,
		long contentId
	) => LogQueue.Add($"""{contentType.Humanize()} ***"{title}"*** (id: {contentId}) has been blocked by **{UserName}**""");

	public void LogContentUnblocked(
		string contentType,
		string title,
		long contentId
	) => LogQueue.Add($"""{contentType.Humanize()} ***"{title}"*** (id: {contentId}) has been unblocked by **{UserName}**""");

	public void LogThreadLocked(
		string contentType,
		long contentId,
		long threadId
	) => LogQueue.Add($"Comment thread for **{contentType}** (id: {contentId}) with the ID **{threadId}** was locked by **{UserName}**");

	public void LogThreadUnlocked(
		string contentType,
		long contentId,
		long threadId
	) => LogQueue.Add($"Comment thread for **{contentType}** (id: {contentId}) with the ID **{threadId}** was unlocked by **{UserName}**");

	// User management
	public void LogUserCreated(long userId, string userName)
		=> LogQueue.Add($"User **{userName}** (id: {userId}) was created by **{UserName}**");

	public void LogUserImpersonated(long targetUserId, string targetUserName)
		=> LogQueue.Add($"User **{targetUserName}** (id: {targetUserId}) is being impersonated by **{UserName}**");

	// News
	public void LogNewsCreated(long newsId, string title)
		=> LogQueue.Add($"""News post ***"{title}"*** (id: {newsId}) was created by **{UserName}**""");

	public void LogNewsUpdated(long newsId, string title)
		=> LogQueue.Add($"""News post ***"{title}"*** (id: {newsId}) was updated by **{UserName}**""");

	// Documents
	public void LogDocumentCreated(string slug, string title)
		=> LogQueue.Add($"""Document ***"{title}"*** (slug: {slug}) was created by **{UserName}**""");

	public void LogDocumentUpdated(string slug, string title, uint version)
		=> LogQueue.Add($"""Document ***"{title}"*** (slug: {slug}) was updated to version {version} by **{UserName}**""");

	// Tags
	public void LogTagCreated(long tagId, string name, string namespaceName)
		=> LogQueue.Add($"Tag **{name}** (id: {tagId}) in namespace **{namespaceName}** was created by **{UserName}**");

	public void LogTagUpdated(long tagId, string name, string namespaceName)
		=> LogQueue.Add($"Tag **{name}** (id: {tagId}) in namespace **{namespaceName}** was updated by **{UserName}**");

	public void LogTagDeleted(long tagId, string name, string namespaceName)
		=> LogQueue.Add($"Tag **{name}** (id: {tagId}) in namespace **{namespaceName}** was deleted by **{UserName}**");

	// Quotes
	public void LogQuoteCreated(long quoteId, string author)
		=> LogQueue.Add($"Quote by **{author}** (id: {quoteId}) was created by **{UserName}**");

	public void LogQuoteUpdated(long quoteId, string author)
		=> LogQueue.Add($"Quote by **{author}** (id: {quoteId}) was updated by **{UserName}**");

	public void LogQuoteDeleted(long quoteId, string author)
		=> LogQueue.Add($"Quote by **{author}** (id: {quoteId}) was deleted by **{UserName}**");

	// Ratings
	public void LogRatingCreated(long ratingId, string name)
		=> LogQueue.Add($"Rating **{name}** (id: {ratingId}) was created by **{UserName}**");

	public void LogRatingUpdated(long ratingId, string name)
		=> LogQueue.Add($"Rating **{name}** (id: {ratingId}) was updated by **{UserName}**");

	public void LogRatingDeleted(long ratingId, string name)
		=> LogQueue.Add($"Rating **{name}** (id: {ratingId}) was deleted by **{UserName}**");

	// FAQs
	public void LogFaqCreated(long faqId, string question)
		=> LogQueue.Add($"FAQ **{question}** (id: {faqId}) was created by **{UserName}**");

	public void LogFaqUpdated(long faqId, string question)
		=> LogQueue.Add($"FAQ **{question}** (id: {faqId}) was updated by **{UserName}**");

	public void LogFaqDeleted(long faqId, string question)
		=> LogQueue.Add($"FAQ **{question}** (id: {faqId}) was deleted by **{UserName}**");

	// Roles
	public void LogTagNamespaceCreated(long namespaceId, string name, string slug, string? alias)
		=> LogQueue.Add($"Tag namespace **{name}** (id: {namespaceId}, slug: **{slug}**, alias: {alias ?? "none"}) was created by **{UserName}**");

	public void LogTagNamespaceUpdated(long namespaceId, string name, string slug, string? alias)
		=> LogQueue.Add($"Tag namespace **{name}** (id: {namespaceId}, slug: **{slug}**, alias: {alias ?? "none"}) was updated by **{UserName}**");

	public void LogTagNamespaceDeleted(long namespaceId, string name, string slug)
		=> LogQueue.Add($"Tag namespace **{name}** (id: {namespaceId}, slug: **{slug}**) was deleted by **{UserName}**");

	public void LogRoleCreated(long roleId, string name, bool isStaff)
		=> LogQueue.Add($"Role **{name}** (id: {roleId}, staff: {isStaff}) was created by **{UserName}**");

	public void LogRoleUpdated(long roleId, string name, bool isStaff)
		=> LogQueue.Add($"Role **{name}** (id: {roleId}, staff: {isStaff}) was updated by **{UserName}**");

	public void LogRoleDeleted(long roleId, string name)
		=> LogQueue.Add($"Role **{name}** (id: {roleId}) was deleted by **{UserName}**");

	// Invite code created
	public void LogInviteCodeCreated()
		=> LogQueue.Add($"Admin Invite code was created by **{UserName}**");
}