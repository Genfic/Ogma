using System.Linq.Expressions;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Ogma3.Data;
using Ogma3.Data.Stories;
using Ogma3.Pages.Shared.Cards;
using ZiggyCreatures.Caching.Fusion;

namespace Ogma3.Pages;

public sealed class IndexModel(ApplicationDbContext context, IFusionCache cache, ILogger<IndexModel> logger)
	: PageModel
{
	public required List<StoryCard> RecentStories { get; set; }
	public required List<StoryCard> TopStories { get; set; }
	public required List<StoryCard> LastUpdatedStories { get; set; }

	public async Task OnGetAsync()
	{
		var shortExpiry = TimeSpan.FromMinutes(5);
		var longExpiry = TimeSpan.FromHours(1);

		// Try getting recent stories from the cache
		RecentStories = await cache.GetOrSetAsync("IndexRecent", async ct => {
			logger.LogInformation("{Stories} cache miss!", nameof(RecentStories));
			return await GetTopStoryCards(10, static s => s.PublicationDate, ct);
		}, options => options.Duration = shortExpiry);

		// Try getting top stories from the cache
		TopStories = await cache.GetOrSetAsync("IndexTop", async ct => {
			logger.LogInformation("{Stories} cache miss!", nameof(TopStories));
			return await GetTopStoryCards(10, static s => s.VoteCount, ct);
		}, options => options.Duration = longExpiry);

		// Try getting recently updated stories from the cache
		LastUpdatedStories = await cache.GetOrSetAsync("IndexUpdated", async ct => {
			logger.LogInformation("{Stories} cache miss!", nameof(LastUpdatedStories));
			return await GetTopStoryCards(10, static s => s.LastUpdatedAt, ct);
		}, options => options.Duration = shortExpiry);
	}

	private async Task<List<StoryCard>> GetTopStoryCards<TProp>(
		int count,
		Expression<Func<Story, TProp>> sort,
		CancellationToken cancellationToken = default
	)
	{
		return await context.Stories
			.TagWith($"{nameof(GetTopStoryCards)} -> {count}")
			.Where(b => b.IsVisible)
			.Where(b => b.ContentBlockId == null)
			.Where(s => s.Rating.BlacklistedByDefault == false)
			.OrderByDescending(sort)
			.Take(count)
			.Select(StoryMapper.MapToCard)
			.ToListAsync(cancellationToken);
	}
}