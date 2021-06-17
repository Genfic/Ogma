using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Caching.Memory;
using Ogma3.Data.Stories;
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

        public List<StoryCard> RecentStories { get; private set; }
        public List<StoryCard> TopStories { get; private set; }
        public List<StoryCard> LastUpdatedStories { get; private set; }
        public async Task OnGetAsync()
        {
            var shortExpiry = TimeSpan.FromMinutes(5);
            var longExpiry = TimeSpan.FromHours(1);
            
            // Try getting recent stories from cache
            const string recentKey = "IndexRecent";
            if (_cache.TryGetValue(recentKey, out List<StoryCard> recent))
            {
                RecentStories = recent;
            }
            else
            {
                RecentStories = await _storiesRepo.GetTopStoryCards(10);
                _cache.Set(recentKey, RecentStories, shortExpiry);
            }
            
            // Try getting top stories from cache
            const string topKey = "IndexTop";
            if (_cache.TryGetValue(topKey, out List<StoryCard> top))
            {
                TopStories = top;
            }
            else
            {
                TopStories = await _storiesRepo.GetTopStoryCards(10, EStorySortingOptions.ScoreDescending);
                _cache.Set(topKey, TopStories, longExpiry);
            }
            
            // Try getting recently updated stories from cache
            const string updatedKey = "IndexUpdated";
            if (_cache.TryGetValue(updatedKey, out List<StoryCard> updated))
            {
                LastUpdatedStories = updated;
            }
            else
            {
                LastUpdatedStories = await _storiesRepo.GetTopStoryCards(10, EStorySortingOptions.UpdatedDescending);
                _cache.Set(updatedKey, LastUpdatedStories, shortExpiry);
            }
            
        }
    }
}
