using Microsoft.EntityFrameworkCore;
using Ogma3.Data;

namespace Ogma3.Infrastructure.Jobs;

public sealed class PublishScheduledContentRecurringJob
(
	IServiceProvider serviceProvider,
	ILogger<PublishScheduledContentRecurringJob> logger
) : BaseRecurringJob(serviceProvider, logger)
{
	protected override TimeSpan Interval => TimeSpan.FromMinutes(1);
	protected override string Name => nameof(PublishScheduledContentRecurringJob);

	protected override async Task Run(CancellationToken ct)
	{
		using var scope = ServiceProvider.CreateScope();
		var ctx = scope.ServiceProvider.GetRequiredService<AppDbContext>();

		var now = DateTimeOffset.UtcNow;
		var cutoff = now - TimeSpan.FromSeconds(30);

		using var loggerScope = logger.BeginScope("Running scheduled publishing for period {Start} - {End}", cutoff, now);

		var postRows = await ctx.Blogposts
			.Where(p => p.PublicationDate == null)
			.Where(p => p.ScheduledFor < cutoff)
			.ExecuteUpdateAsync(setters => setters
				.SetProperty(p => p.PublicationDate, now)
				.SetProperty(p => p.IsVisible, true), ct);

		if (postRows > 0)
		{
			logger.LogInformation("Published {PostCount} scheduled blogposts", postRows);
		}

		var storyRows = await ctx.Stories
			.Where(s => s.PublicationDate == null)
			.Where(s => s.ScheduledFor < cutoff)
			.Where(s => s.ChapterCount > 0)
			.ExecuteUpdateAsync(setters => setters
				.SetProperty(s => s.PublicationDate, now)
				.SetProperty(s => s.IsVisible, true), ct);

		if (storyRows > 0)
		{
			logger.LogInformation("Published {StoryCount} scheduled stories", storyRows);
		}

		var chapterRows = await ctx.Chapters
			.Where(c => c.PublicationDate == null)
			.Where(c => c.ScheduledFor < cutoff)
			.ExecuteUpdateAsync(setters => setters
				.SetProperty(c => c.PublicationDate, now)
				.SetProperty(c => c.IsVisible, true), ct);

		if (chapterRows > 0)
		{
			logger.LogInformation("Published {ChapterCount} scheduled chapters", chapterRows);
		}
	}
}