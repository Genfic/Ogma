using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Ogma3.Data;
using Ogma3.Data.Ratings;
using Ogma3.Data.Stories;
using Ogma3.Pages.Shared;
using Ogma3.Pages.Shared.Cards;

namespace Ogma3.Pages.Stories
{
    public class IndexModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly StoriesRepository _storiesRepo;
        private readonly OgmaConfig _config;
        public IndexModel(ApplicationDbContext context, StoriesRepository storiesRepo, OgmaConfig config)
        {
            _context = context;
            _storiesRepo = storiesRepo;
            _config = config;
        }

        public List<Rating> Ratings { get; set; }
        public List<StoryCard> Stories { get; set; }
        public IEnumerable<long> Tags { get; set; }
        public EStorySortingOptions SortBy { get; set; }
        public string SearchBy { get; set; }
        public long? Rating { get; set; }
        public Pagination Pagination { get; set; }
        
        public async Task OnGetAsync(
            [FromQuery] IList<long> tags,
            [FromQuery] string q = null, 
            [FromQuery] EStorySortingOptions sort = EStorySortingOptions.DateDescending,
            [FromQuery] long? rating = null,
            [FromQuery] int page = 1
        )
        {
            SearchBy = q;
            SortBy = sort;
            Rating = rating;
            Tags = tags;
            
            // Load ratings
            Ratings = await _context.Ratings.ToListAsync();

            // Load stories
            Stories = await _storiesRepo.SearchAndSortStoryCards(_config.StoriesPerPage, page, tags, q, rating, sort);
            
            // Prepare pagination
            Pagination = new Pagination
            {
                PerPage = _config.StoriesPerPage,
                ItemCount = await _storiesRepo.CountSearchResults(tags, q, rating),
                CurrentPage = page
            };
        }

    }
}