using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Ogma3.Data;
using Ogma3.Data.Stories;
using Ogma3.Infrastructure.Extensions;
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
			return await GetTopStoryCards(10, cancellationToken: ct);
		}, options => options.Duration = shortExpiry);

		// Try getting top stories from the cache
		TopStories = await cache.GetOrSetAsync("IndexTop", async ct => {
			logger.LogInformation("{Stories} cache miss!", nameof(TopStories));
			return await GetTopStoryCards(10, EStorySortingOptions.ScoreDescending, ct);
		}, options => options.Duration = longExpiry);

		// Try getting recently updated stories from the cache
		LastUpdatedStories = await cache.GetOrSetAsync("IndexUpdated", async ct => {
			logger.LogInformation("{Stories} cache miss!", nameof(LastUpdatedStories));
			return await GetTopStoryCards(10, EStorySortingOptions.UpdatedDescending, ct);
		}, options => options.Duration = shortExpiry);
	}

	private async Task<List<StoryCard>> GetTopStoryCards(
		int count,
		EStorySortingOptions sort = EStorySortingOptions.DateDescending,
		CancellationToken cancellationToken = default
	)
	{
		var userId = User.GetNumericId();

		return await context.Stories
			.TagWith($"{nameof(GetTopStoryCards)} -> {count}, {sort}")
			.Where(b => b.PublicationDate != null)
			.Where(b => b.ContentBlockId == null)
			.Blacklist(context, userId)
			.SortByEnum(sort)
			.Take(count)
			.Select(StoryMapper.MapToCard)
			.ToListAsync(cancellationToken);
	}
}