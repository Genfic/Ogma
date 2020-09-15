using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Ogma3.Data.Enums;
using Ogma3.Data.Repositories;
using Ogma3.Pages.Shared;

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
        public async Task OnGetAsync()
        {
            RecentStories = await _storiesRepo.GetAndSortPaginatedStoryCards(sort: EStorySortingOptions.DateDescending);
            TopStories = await _storiesRepo.GetAndSortPaginatedStoryCards(sort: EStorySortingOptions.ScoreDescending);
        }
    }
}
