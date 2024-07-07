using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Ogma3.Data;
using Ogma3.Data.Stories;
using Ogma3.Infrastructure.Extensions;
using Ogma3.Pages.Shared.Cards;

namespace Ogma3.Pages;

public class IndexModel(ApplicationDbContext context, IMemoryCache cache, ILogger<IndexModel> logger)
	: PageModel
{
	public required List<StoryCard> RecentStories { get; set; }
	public required List<StoryCard> TopStories { get; set; }
	public required List<StoryCard> LastUpdatedStories { get; set; }

	public async Task OnGetAsync()
	{
		var shortExpiry = TimeSpan.FromMinutes(5);
		var longExpiry = TimeSpan.FromHours(1);
		var uid = User.GetNumericId();

		// Try getting recent stories from cache
		RecentStories = await cache.GetOrCreateAsync("IndexRecent", async entry =>
		{
			logger.LogInformation("{Stories} cache miss!", nameof(RecentStories));
			entry.AbsoluteExpirationRelativeToNow = shortExpiry;
			return await GetTopStoryCards(10, uid);
		}) ?? [];

		// Try getting top stories from cache
		TopStories = await cache.GetOrCreateAsync("IndexTop", async entry =>
		{
			logger.LogInformation("{Stories} cache miss!", nameof(TopStories));
			entry.AbsoluteExpirationRelativeToNow = longExpiry;
			return await GetTopStoryCards(10, uid, EStorySortingOptions.ScoreDescending);
		}) ?? [];

		// Try getting recently updated stories from cache
		LastUpdatedStories = await cache.GetOrCreateAsync("IndexUpdated", async entry =>
		{
			logger.LogInformation("{Stories} cache miss!", nameof(LastUpdatedStories));
			entry.AbsoluteExpirationRelativeToNow = shortExpiry;
			return await GetTopStoryCards(10, uid, EStorySortingOptions.UpdatedDescending);
		}) ?? [];
	}
	
	private async Task<List<StoryCard>> GetTopStoryCards(int count, long? userId, EStorySortingOptions sort = EStorySortingOptions.DateDescending)
	{
		return await context.Stories
			.TagWith($"{nameof(GetTopStoryCards)} -> {count}, {sort}")
			.Where(b => b.PublicationDate != null)
			.Where(b => b.ContentBlockId == null)
			.Blacklist(context, userId)
			.SortByEnum(sort)
			.Take(count)
			.ProjectToCard()
			.ToListAsync();
	}
}