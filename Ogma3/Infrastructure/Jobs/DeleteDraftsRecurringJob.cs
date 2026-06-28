using Microsoft.EntityFrameworkCore;
using Ogma3.Data;
using Ogma3.Data.Subscriptions;

namespace Ogma3.Infrastructure.Jobs;

public sealed class DeleteDraftsRecurringJob
	(IServiceProvider serviceProvider, OgmaConfig.OgmaConfig config, ILogger<DeleteDraftsRecurringJob> logger)
	: BaseRecurringJob(serviceProvider, logger)
{
	protected override TimeSpan Interval => TimeSpan.FromDays(1);
	protected override string Name => nameof(DeleteDraftsRecurringJob);

	protected override async Task Run(CancellationToken ct)
	{
		using var scope = ServiceProvider.CreateScope();
		var ctx = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

		var safeUsers = ctx.Subscriptions
			.Where(s => s.Tier != null && (s.Tier.Entitlements & Entitlement.DraftsLastForever) != 0)
			.Select(s => s.UserId);

		var longerUsers = ctx.Subscriptions
			.Where(s => s.Tier != null && (s.Tier.Entitlements & Entitlement.DraftsLastLonger) != 0)
			.Select(s => s.UserId);

		var now = DateTimeOffset.UtcNow;
		var chapterCount = await ctx.Chapters
			.Where(c => c.PublicationDate == null)
			.Where(c => !safeUsers.Contains(c.Story.AuthorId))
			.Where(c => c.CreationDate < (longerUsers.Contains(c.Story.AuthorId)
				? now.AddDays(-config.PremiumDraftRetentionDays)
				: now.AddDays(-config.DraftRetentionDays)))
			.ExecuteDeleteAsync(ct);

		logger.LogInformation("Deleted {ChapterCount} chapter drafts", chapterCount);

		var blogpostCount = await ctx.Blogposts
			.Where(b => b.PublicationDate == null)
			.Where(b => !safeUsers.Contains(b.AuthorId))
			.Where(b => b.CreationDate < (longerUsers.Contains(b.AuthorId)
				? now.AddDays(-config.PremiumDraftRetentionDays)
				: now.AddDays(-config.DraftRetentionDays)))
			.ExecuteDeleteAsync(ct);

		logger.LogInformation("Deleted {BlogpostCount} blogpost drafts", blogpostCount);
	}
}