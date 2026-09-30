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
		var ctx = scope.ServiceProvider.GetRequiredService<AppDbContext>();

		var now = DateTimeOffset.UtcNow;

		var active = ctx.Subscriptions
			.Where(SubscriptionEntitlements.IsActive(TimeSpan.FromDays(config.EntitlementGraceDays), now))
			.Where(s => s.Tier != null);

		var safeUsers = active
			.Where(s => (s.Tier!.Entitlements & Entitlement.DraftsLastForever) != 0)
			.Select(s => s.UserId);

		var longerUsers = active
			.Where(s => (s.Tier!.Entitlements & Entitlement.DraftsLastLonger) != 0)
			.Select(s => s.UserId);

		var doomedChapters = ctx.Chapters
			.Where(c => c.PublicationDate == null)
			.Where(c => !safeUsers.Contains(c.Story.AuthorId))
			.Where(c => c.CreationDate < (longerUsers.Contains(c.Story.AuthorId)
				? now.AddDays(-config.PremiumDraftRetentionDays)
				: now.AddDays(-config.DraftRetentionDays)));

		await using var transaction = await ctx.Database.BeginTransactionAsync(ct);

		// The aggregate has to be read up front: `ExecuteDeleteAsync` bypasses change tracking, so
		// the parent story's denormalized counters would otherwise drift upwards forever.
		var perStory = await doomedChapters
			.GroupBy(c => c.StoryId)
			.Select(g => new
			{
				StoryId = g.Key,
				Chapters = g.Count(),
				Words = g.Sum(c => c.WordCount),
			})
			.ToListAsync(ct);

		var chapterCount = await doomedChapters.ExecuteDeleteAsync(ct);

		foreach (var story in perStory)
		{
			await ctx.Stories
				.Where(s => s.Id == story.StoryId)
				.ExecuteUpdateAsync(setters => setters
					.SetProperty(s => s.ChapterCount, s => s.ChapterCount - story.Chapters < 0 ? 0 : s.ChapterCount - story.Chapters)
					.SetProperty(s => s.WordCount, s => s.WordCount - story.Words < 0 ? 0 : s.WordCount - story.Words), ct);
		}

		await transaction.CommitAsync(ct);

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