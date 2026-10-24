using Microsoft.EntityFrameworkCore;

namespace Ogma3.Data.Stories;

public static class StoryCountExtensions
{
	extension(AppDbContext ctx)
	{
		/// <summary>
		/// Recomputes <see cref="Story.ChapterCount"/> and <see cref="Story.WordCount"/> from the chapters a
		/// regular visitor can see, and returns the number of stories updated.
		/// </summary>
		/// <param name="storyId">The story to recompute. A no-op if the story is itself soft-deleted.</param>
		/// <param name="touchLastUpdatedAt">
		/// Set this only when the public content actually changed. <see cref="Story.LastUpdatedAt"/> drives
		/// the "recently updated" feeds and the story index ordering, so housekeeping such as purging drafts
		/// must not bump it.
		/// </param>
		/// <param name="ct">Cancellation token.</param>
		public Task<int> RecalculateChapterCounts(
			long storyId,
			bool touchLastUpdatedAt = false,
			CancellationToken ct = default
		) => ctx.Stories
			.Where(s => s.Id == storyId)
			.ExecuteUpdateAsync(s => s
				.SetProperty(x => x.ChapterCount, x => x.Chapters
					.Count(c => c.IsVisible && c.ContentBlockId == null && c.ScheduledForDeletion == null))
				.SetProperty(x => x.WordCount, x => x.Chapters
					.Where(c => c.IsVisible && c.ContentBlockId == null && c.ScheduledForDeletion == null)
					.Select(c => (int?)c.WordCount)
					.Sum() ?? 0)
				.SetProperty(x => x.LastUpdatedAt, x => touchLastUpdatedAt ? DateTimeOffset.UtcNow : x.LastUpdatedAt),
				ct);
	}
}
