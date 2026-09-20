using Microsoft.EntityFrameworkCore;
using Ogma3.Data;
using Ogma3.Infrastructure.Logging.OperationTiming;
using Ogma3.Services.Mailer;
using Ogma3.Services.FileUploader;

namespace Ogma3.Infrastructure.Jobs;

public sealed class ProcessSoftDeletesRecurringJob
(
	IServiceProvider serviceProvider,
	ILogger<ProcessSoftDeletesRecurringJob> logger)
	: BaseRecurringJob(serviceProvider, logger)
{
	protected override TimeSpan Interval => TimeSpan.FromHours(1);
	protected override string Name => nameof(ProcessSoftDeletesRecurringJob);

	private readonly List<BulkEmail> _emails = [];

	protected override async Task Run(CancellationToken ct)
	{
		using var op = logger.TimeOperation("Processing soft deletes");

		using var scope = ServiceProvider.CreateScope();
		var ctx = scope.ServiceProvider.GetRequiredService<AppDbContext>();
		var mailer = scope.ServiceProvider.GetRequiredService<IMailer>();

		var now = DateTimeOffset.UtcNow;
		var cutoff = now.AddDays(-7);

		// Process stories
		var storiesToDelete = await ctx.Stories
			.IgnoreQueryFilters()
			.Where(s => s.ScheduledForDeletion != null && s.ScheduledForDeletion <= cutoff)
			.Include(s => s.Author)
			.Include(s => s.Cover)
			.ToListAsync(ct);

		foreach (var story in storiesToDelete)
		{
			try
			{
				// Delete cover if exists
				if (story.Cover is { ETag: not null })
				{
					using var fileScope = ServiceProvider.CreateScope();
					var uploader = fileScope.ServiceProvider.GetRequiredService<IFileUploader>();
					await uploader.Delete(story.Cover.Url, ct);
				}

				ctx.Stories.Remove(story);
				logger.LogInformation("Deleted story {StoryId} ({Title}) scheduled for {ScheduledFor}", story.Id, story.Title,
					story.ScheduledForDeletion);

				QueueEmail(story.Author.Email, story.Author.UserName, story.Title, "story");
			}
			catch (Exception)
			{
				logger.LogError("Error deleting story {StoryId}", story.Id);
			}
		}

		// Process chapters
		var chaptersToDelete = await ctx.Chapters
			.IgnoreQueryFilters()
			.Where(c => c.ScheduledForDeletion != null && c.ScheduledForDeletion <= cutoff)
			.Include(c => c.Story)
			.ThenInclude(s => s.Author)
			.ToListAsync(ct);

		foreach (var chapter in chaptersToDelete)
		{
			try
			{
				ctx.Chapters.Remove(chapter);
				logger.LogInformation("Deleted chapter {ChapterId} ({Title}) scheduled for {ScheduledFor}", chapter.Id, chapter.Title,
					chapter.ScheduledForDeletion);

				QueueEmail(chapter.Story.Author.Email, chapter.Story.Author.UserName, chapter.Title, "chapter", chapter.Story.Title);
			}
			catch (Exception)
			{
				logger.LogError("Error deleting chapter {ChapterId}", chapter.Id);
			}
		}

		// Process blogposts
		var blogpostsToDelete = await ctx.Blogposts
			.IgnoreQueryFilters()
			.Where(b => b.ScheduledForDeletion != null && b.ScheduledForDeletion <= cutoff)
			.Include(b => b.Author)
			.ToListAsync(ct);

		foreach (var blogpost in blogpostsToDelete)
		{
			try
			{
				ctx.Blogposts.Remove(blogpost);
				logger.LogInformation("Deleted blogpost {BlogpostId} ({Title}) scheduled for {ScheduledFor}", blogpost.Id, blogpost.Title,
					blogpost.ScheduledForDeletion);

				QueueEmail(blogpost.Author.Email, blogpost.Author.UserName, blogpost.Title, "blogpost");
			}
			catch (Exception)
			{
				logger.LogError("Error deleting blogpost {BlogpostId}", blogpost.Id);
			}
		}

		if (storiesToDelete.Count > 0 || chaptersToDelete.Count > 0 || blogpostsToDelete.Count > 0)
		{
			try
			{
				await ctx.SaveChangesAsync(ct);

				foreach (var chunk in _emails.Chunk(250))
				{
					await mailer.SendBulkEmailTemplateAsync([..chunk]);
				}
			}
			finally
			{
				_emails.Clear();
			}
		}
	}

	private void QueueEmail(string email, string username, string title, string contentType, string? parentTitle = null)
	{
		var model = new Dictionary<string, string>
		{
			["name"] = username,
			["content_title"] = parentTitle is null ? title : $"{parentTitle}: {title}",
			["content_type"] = contentType,
		};
		_emails.Add(new BulkEmail(email, "content-deleted", model));
	}
}