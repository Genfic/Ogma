using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Ogma3.Data.Stories;
using Ogma3.Pages.Shared.Cards;

namespace Ogma3.Pages;

public class IndexModel(StoriesRepository storiesRepo, IMemoryCache cache, ILogger<IndexModel> logger)
	: PageModel
{
	public required List<StoryCard> RecentStories { get; set; }
	public required List<StoryCard> TopStories { get; set; }
	public required List<StoryCard> LastUpdatedStories { get; set; }

	public async Task OnGetAsync()
	{
		var shortExpiry = TimeSpan.FromMinutes(5);
		var longExpiry = TimeSpan.FromHours(1);

		// Try getting recent stories from cache
		RecentStories = await cache.GetOrCreateAsync("IndexRecent", async entry =>
		{
			logger.LogInformation("{Stories} cache miss!", nameof(RecentStories));
			entry.AbsoluteExpirationRelativeToNow = shortExpiry;
			return await storiesRepo.GetTopStoryCards(10);
		}) ?? [];

		// Try getting top stories from cache
		TopStories = await cache.GetOrCreateAsync("IndexTop", async entry =>
		{
			logger.LogInformation("{Stories} cache miss!", nameof(TopStories));
			entry.AbsoluteExpirationRelativeToNow = longExpiry;
			return await storiesRepo.GetTopStoryCards(10, EStorySortingOptions.ScoreDescending);
		}) ?? [];

		// Try getting recently updated stories from cache
		LastUpdatedStories = await cache.GetOrCreateAsync("IndexUpdated", async entry =>
		{
			logger.LogInformation("{Stories} cache miss!", nameof(LastUpdatedStories));
			entry.AbsoluteExpirationRelativeToNow = shortExpiry;
			return await storiesRepo.GetTopStoryCards(10, EStorySortingOptions.UpdatedDescending);
		}) ?? [];
	}
}