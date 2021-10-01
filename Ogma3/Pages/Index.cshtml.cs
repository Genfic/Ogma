using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Caching.Memory;
using Ogma3.Data.Stories;
using Ogma3.Pages.Shared.Cards;

namespace Ogma3.Pages;

public class IndexModel : PageModel
{
    private readonly StoriesRepository _storiesRepo;
    private readonly IMemoryCache _cache;

    public IndexModel(StoriesRepository storiesRepo, IMemoryCache cache)
    {
        _storiesRepo = storiesRepo;
        _cache = cache;
    }

    public List<StoryCard> RecentStories { get; private set; }
    public List<StoryCard> TopStories { get; private set; }
    public List<StoryCard> LastUpdatedStories { get; private set; }
    public async Task OnGetAsync()
    {
        var shortExpiry = TimeSpan.FromMinutes(5);
        var longExpiry = TimeSpan.FromHours(1);
            
        // Try getting recent stories from cache
        RecentStories = await _cache.GetOrCreateAsync("IndexRecent", async entry =>
        {
            entry.AbsoluteExpirationRelativeToNow = shortExpiry;
            return await _storiesRepo.GetTopStoryCards(10);
        });
            
        // Try getting top stories from cache
        TopStories = await _cache.GetOrCreateAsync("IndexTop", async entry =>
        {
            entry.AbsoluteExpirationRelativeToNow = longExpiry;
            return await _storiesRepo.GetTopStoryCards(10, EStorySortingOptions.ScoreDescending);
        });
            
        // Try getting recently updated stories from cache
        LastUpdatedStories = await _cache.GetOrCreateAsync("IndexUpdated", async entry =>
        {
            entry.AbsoluteExpirationRelativeToNow = shortExpiry;
            return await _storiesRepo.GetTopStoryCards(10, EStorySortingOptions.UpdatedDescending);
        });

    }
}