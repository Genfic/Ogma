using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Ogma3.Data.Enums;
using Ogma3.Data.Repositories;
using Ogma3.Pages.Shared;
using Ogma3.Pages.Shared.Cards;

namespace Ogma3.Pages
{
    public class IndexModel : PageModel
    {
        private readonly StoriesRepository _storiesRepo;

        public IndexModel(StoriesRepository storiesRepo)
        {
            _storiesRepo = storiesRepo;
        }

        public List<StoryCard> RecentStories { get; set; }
        public List<StoryCard> TopStories { get; set; }
        public List<StoryCard> LastUpdatedStories { get; set; }
        public async Task OnGetAsync()
        {
            RecentStories = await _storiesRepo.GetTopStoryCards(10);
            TopStories = await _storiesRepo.GetTopStoryCards(10, EStorySortingOptions.ScoreDescending);
            LastUpdatedStories = await _storiesRepo.GetTopStoryCards(10, EStorySortingOptions.UpdatedAscending);
        }
    }
}
