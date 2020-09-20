using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Ogma3.Data;
using Ogma3.Data.Repositories;
using Ogma3.Pages.Shared;
using Utils.Extensions;

namespace Ogma3.Pages.Stories
{
    [Authorize]
    public class PublishModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly StoriesRepository _storiesRepo;

        public PublishModel(ApplicationDbContext context, StoriesRepository storiesRepo)
        {
            _context = context;
            _storiesRepo = storiesRepo;
        }

        [BindProperty] 
        public StoryDetails Story { get; set; }

        public async Task<IActionResult> OnGetAsync(long id)
        {
            Story = await _storiesRepo.GetStoryDetails(id);

            if (Story == null) return NotFound();
            if (!User.IsUserSameAsLoggedIn(Story.AuthorId)) return RedirectToPage("Index");

            return Page();
        }

        public async Task<IActionResult> OnPostAsync(long id)
        {
            var story = await _context.Stories
                .Where(s => s.Id == id)
                .Select(s => new {s.Id, s.Slug, AuthorId = s.Author.Id})
                .AsNoTracking()
                .FirstOrDefaultAsync();
            
            if (story == null) return NotFound();
            if (!User.IsUserSameAsLoggedIn(story.AuthorId)) return RedirectToPage("Index");

            await _storiesRepo.TogglePublishedStatus(id);

            return RedirectToPage("../Story", new { id = story.Id, slug = story.Slug });
        }
    }
}