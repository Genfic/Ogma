using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Ogma3.Data.DTOs;
using Ogma3.Data.Repositories;
using Ogma3.Pages.Shared;
using Ogma3.Pages.Shared.Cards;

namespace Ogma3.Pages
{
    public class TagModel : PageModel
    {
        private const int PerPage = 25;
        
        private readonly TagsRepository _tagsRepo;
        private readonly StoriesRepository _storiesRepo;
        
        public TagModel(TagsRepository tagsRepo, StoriesRepository storiesRepo)
        {
            _tagsRepo = tagsRepo;
            _storiesRepo = storiesRepo;
        }

        public TagDto Tag { get; set; }
        public IList<StoryCard> Stories { get; set; }
        public Pagination Pagination { get; set; }

        public async Task<IActionResult> OnGetAsync(long id, string? slug, [FromQuery] int page = 1)
        {
            Tag = await _tagsRepo.GetTag(id);
            if (Tag == null) return NotFound();

            Stories = await _storiesRepo.GetCardsWithTag(id, page, PerPage);

            // Prepare pagination
            Pagination = new Pagination
            {
                CurrentPage = page,
                ItemCount = await _storiesRepo.CountWithTag(id),
                PerPage = PerPage
            };
            
            return Page();
        }
    }
}
