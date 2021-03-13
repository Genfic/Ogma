using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Caching.Memory;
using Ogma3.Data.Enums;
using Ogma3.Data.Repositories;
using Ogma3.Pages.Shared.Cards;

namespace Ogma3.Pages
{
    public class IndexModel : PageModel
    {
        private readonly StoriesRepository _storiesRepo;
        private readonly IMemoryCache _cache;

        public IndexModel(StoriesRepository storiesRepo, IMemoryCache cache)
        {
            _storiesRepo = storiesRepo;
            _cache = cache;
        }

        public List<StoryCard> RecentStories { get; set; }
        public List<StoryCard> TopStories { get; set; }
        public List<StoryCard> LastUpdatedStories { get; set; }
        public async Task OnGetAsync()
        {
            var shortExpiry = TimeSpan.FromMinutes(5);
            var longExpiry = TimeSpan.FromHours(1);
            
            // Try getting recent stories from cache
            const string recentKey = "IndexRecent";
            var recentsCached = _cache.TryGetValue(recentKey, out List<StoryCard> recent);
            if (recentsCached)
                RecentStories = recent;
            else
            {
                Console.WriteLine(">> No Cache: recent");
                RecentStories = await _storiesRepo.GetTopStoryCards(10);
                _cache.Set(recentKey, RecentStories, shortExpiry);
            }
            
            // Try getting top stories from cache
            const string topKey = "IndexTop";
            var topCached = _cache.TryGetValue(topKey, out List<StoryCard> top);
            if (topCached)
                TopStories = top;
            else
            {
                Console.WriteLine(">> No Cache: top");
                TopStories = await _storiesRepo.GetTopStoryCards(10, EStorySortingOptions.ScoreDescending);
                _cache.Set(topKey, TopStories, longExpiry);
            }
            
            // Try getting recently updated stories from cache
            const string updatedKey = "IndexUpdated";
            var updatedCached = _cache.TryGetValue(updatedKey, out List<StoryCard> updated);
            if (updatedCached)
                LastUpdatedStories = updated;
            else
            {
                Console.WriteLine(">> No Cache: updated");
                LastUpdatedStories = await _storiesRepo.GetTopStoryCards(10, EStorySortingOptions.UpdatedDescending);
                _cache.Set(updatedKey, LastUpdatedStories, shortExpiry);
            }
            
        }
    }
}
