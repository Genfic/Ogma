using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Ogma3.Data;
using Ogma3.Data.DTOs;
using Ogma3.Data.Models;
using Ogma3.Data.Repositories;
using Ogma3.Pages.Shared;

namespace Ogma3.Pages
{
    public class TagModel : PageModel
    {
        private readonly TagsRepository _tagsRepo;
        private readonly StoriesRepository _storiesRepo;
        
        public TagModel(TagsRepository tagsRepo, StoriesRepository storiesRepo)
        {
            _tagsRepo = tagsRepo;
            _storiesRepo = storiesRepo;
        }

        public TagDto Tag { get; set; }
        public IList<StoryCard> Stories { get; set; }

        public async Task<IActionResult> OnGetAsync(long id, string? slug)
        {
            Tag = await _tagsRepo.GetTag(id);
            
            if (Tag == null) return NotFound();

            Stories = await _storiesRepo.SearchAndSortStoryCards(tags: new List<long>{ Tag.Id });

            return Page();
        }
    }
}
