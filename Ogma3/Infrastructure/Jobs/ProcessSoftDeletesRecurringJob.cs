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

	private const int EmailChunkSize = 250;

	protected override async Task Run(CancellationToken ct)
	{
		using var op = logger.TimeOperation("Processing soft deletes");

		using var scope = ServiceProvider.CreateScope();
		var ctx = scope.ServiceProvider.GetRequiredService<AppDbContext>();
		var mailer = scope.ServiceProvider.GetRequiredService<IMailer>();

		// Content is scheduled for deletion exactly 7 days out, so by the time a row
		// reaches this cutoff its grace period has already elapsed.
		var cutoff = DateTimeOffset.UtcNow;

		var ids = await ctx.Database.SqlQuery<ResultRow>( // lang=sql
				$"""
				 SELECT "Id", {(byte)ContentType.Story} as "Type" FROM "Stories" WHERE "ScheduledForDeletion" IS NOT NULL AND "ScheduledForDeletion" <= {cutoff}
				 UNION ALL
				 SELECT "Id", {(byte)ContentType.Chapter} as "Type" FROM "Chapters" WHERE "ScheduledForDeletion" IS NOT NULL AND "ScheduledForDeletion" <= {cutoff}
				 UNION ALL
				 SELECT "Id", {(byte)ContentType.Blogpost} as "Type" FROM "Blogposts" WHERE "ScheduledForDeletion" IS NOT NULL AND "ScheduledForDeletion" <= {cutoff}
				 """)
			.ToListAsync(ct);

		if (ids.Count <= 0)
		{
			return;
		}

		var groups = ids
			.GroupBy(r => r.Type)
			.Where(g => g.Any())
			.ToDictionary(
				g => g.Key,
				g => g.Select(x => x.Id).ToList()
			);

		var emails = new List<BulkEmail>();
		List<string>? coversToDelete = null;

		if (groups.TryGetValue(ContentType.Story, out var storyIds))
		{
			coversToDelete = await ProcessStories(ctx, storyIds, emails, ct);
		}

		if (groups.TryGetValue(ContentType.Chapter, out var chapterIds))
		{
			await ProcessChapters(ctx, chapterIds, emails, ct);
		}

		if (groups.TryGetValue(ContentType.Blogpost, out var blogpostIds))
		{
			await ProcessBlogposts(ctx, blogpostIds, emails, ct);
		}

		// Covers are removed only after the deletion is committed, so a failure here can never
		// leave a surviving story pointing at an object that is already gone from storage.
		if (coversToDelete is { Count: > 0 })
		{
			using var fileScope = ServiceProvider.CreateScope();
			var uploader = fileScope.ServiceProvider.GetRequiredService<IFileUploader>();

			foreach (var coverUrl in coversToDelete)
			{
				try
				{
					await uploader.Delete(coverUrl, ct);
				}
				catch (Exception e)
				{
					logger.LogError(e, "Failed to delete cover {CoverUrl} of a deleted story", coverUrl);
				}
			}
		}

		// The content is already gone at this point, so a failed batch must not abandon the rest.
		foreach (var chunk in emails.Chunk(EmailChunkSize))
		{
			try
			{
				await mailer.SendBulkEmailTemplateAsync([.. chunk]);
			}
			catch (Exception e)
			{
				logger.LogError(e, "Failed to send {Count} content-deleted notifications", chunk.Length);
			}
		}
	}

	private async Task<List<string>?> ProcessStories(AppDbContext ctx, List<long> ids, List<BulkEmail> emails, CancellationToken ct)
	{
		await using var transaction = await ctx.Database.BeginTransactionAsync(ct);

		var storiesToDelete = await ctx.Stories
			.IgnoreQueryFilters()
			.Where(s => ids.Contains(s.Id))
			.Select(s => new
			{
				s.Id,
				s.Title,
				HasCover = s.Cover != null && s.Cover.ETag != null,
				CoverUrl = s.Cover == null ? null : s.Cover.Url,
				AuthorEmail = s.Author.Email,
				AuthorUserName = s.Author.UserName,
				s.ScheduledForDeletion,
			})
			.ToListAsync(ct);

		if (storiesToDelete.Count <= 0)
		{
			return null;
		}

		// Rows scheduled for deletion are hidden by the global query filter, so it has to be
		// bypassed here – without it this deletes nothing and the transaction is rolled back.
		var rows = await ctx.Stories
			.IgnoreQueryFilters()
			.Where(s => storiesToDelete.Select(sd => sd.Id).Contains(s.Id))
			.ExecuteDeleteAsync(ct);

		if (rows != storiesToDelete.Count)
		{
			await transaction.RollbackAsync(ct);
			return null;
		}

		List<string>? covers = null;
		foreach (var story in storiesToDelete)
		{
			if (story.HasCover && story.CoverUrl is not null)
			{
				(covers ??= []).Add(story.CoverUrl);
			}
			logger.LogInformation("Deleted story {StoryId} ({Title}) scheduled for {ScheduledFor}", story.Id, story.Title,
				story.ScheduledForDeletion);

			QueueEmail(emails, story.AuthorEmail, story.AuthorUserName, story.Title, "story");
		}

		await transaction.CommitAsync(ct);

		return covers;
	}

	private async Task ProcessChapters(AppDbContext ctx, List<long> ids, List<BulkEmail> emails, CancellationToken ct)
	{
		await using var transaction = await ctx.Database.BeginTransactionAsync(ct);

		var chaptersToDelete = await ctx.Chapters
			.IgnoreQueryFilters()
			.Where(s => ids.Contains(s.Id))
			.Select(s => new
			{
				s.Id,
				s.Title,
				StoryTitle = s.Story.Title,
				AuthorEmail = s.Story.Author.Email,
				AuthorUserName = s.Story.Author.UserName,
				s.ScheduledForDeletion,
			})
			.ToListAsync(ct);

		if (chaptersToDelete.Count <= 0)
		{
			return;
		}

		// Rows scheduled for deletion are hidden by the global query filter, so it has to be
		// bypassed here – without it this deletes nothing and the transaction is rolled back.
		var rows = await ctx.Chapters
			.IgnoreQueryFilters()
			.Where(s => chaptersToDelete.Select(sd => sd.Id).Contains(s.Id))
			.ExecuteDeleteAsync(ct);

		if (rows != chaptersToDelete.Count)
		{
			await transaction.RollbackAsync(ct);
			return;
		}

		foreach (var chapter in chaptersToDelete)
		{
			logger.LogInformation("Deleted chapter {ChapterId} ({Title}) scheduled for {ScheduledFor}", chapter.Id, chapter.Title,
				chapter.ScheduledForDeletion);
			QueueEmail(emails, chapter.AuthorEmail, chapter.AuthorUserName, chapter.Title, "chapter", chapter.StoryTitle);
		}

		await transaction.CommitAsync(ct);
	}

	private async Task ProcessBlogposts(AppDbContext ctx, List<long> ids, List<BulkEmail> emails, CancellationToken ct)
	{
		await using var transaction = await ctx.Database.BeginTransactionAsync(ct);

		var blogpostsToDelete = await ctx.Blogposts
			.IgnoreQueryFilters()
			.Where(s => ids.Contains(s.Id))
			.Select(s => new
			{
				s.Id,
				s.Title,
				AuthorEmail = s.Author.Email,
				AuthorUserName = s.Author.UserName,
				s.ScheduledForDeletion,
			})
			.ToListAsync(ct);

		if (blogpostsToDelete.Count <= 0)
		{
			return;
		}

		// Rows scheduled for deletion are hidden by the global query filter, so it has to be
		// bypassed here – without it this deletes nothing and the transaction is rolled back.
		var rows = await ctx.Blogposts
			.IgnoreQueryFilters()
			.Where(s => blogpostsToDelete.Select(sd => sd.Id).Contains(s.Id))
			.ExecuteDeleteAsync(ct);

		if (rows != blogpostsToDelete.Count)
		{
			await transaction.RollbackAsync(ct);
			return;
		}

		foreach (var blogpost in blogpostsToDelete)
		{
			logger.LogInformation("Deleted blogpost {BlogpostId} ({Title}) scheduled for {ScheduledFor}", blogpost.Id, blogpost.Title,
				blogpost.ScheduledForDeletion);
			QueueEmail(emails, blogpost.AuthorEmail, blogpost.AuthorUserName, blogpost.Title, "blogpost");
		}

		await transaction.CommitAsync(ct);
	}

	private static void QueueEmail(List<BulkEmail> emails, string email, string username, string title, string contentType, string? parentTitle = null)
	{
		var model = new Dictionary<string, string>
		{
			["name"] = username,
			["content_title"] = parentTitle is null ? title : $"{parentTitle}: {title}",
			["content_type"] = contentType,
		};
		emails.Add(new BulkEmail(email, "content-deleted", model));
	}

	private sealed record ResultRow(long Id, ContentType Type);
}