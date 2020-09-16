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

namespace Ogma3.Pages.Blog
{
    [Authorize]
    public class PublishModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly BlogpostsRepository _blogpostsRepo;

        public PublishModel(ApplicationDbContext context, BlogpostsRepository blogpostsRepo)
        {
            _context = context;
            _blogpostsRepo = blogpostsRepo;
        }

        [BindProperty] 
        public BlogpostDetails Blogpost { get; set; }

        public async Task<IActionResult> OnGetAsync(long id)
        {
            Blogpost = await _blogpostsRepo.Get(id);

            if (Blogpost == null) return NotFound();
            if (!User.IsUserSameAsLoggedIn(Blogpost.AuthorId)) return RedirectToPage("Index");

            return Page();
        }

        public async Task<IActionResult> OnPostAsync(long id)
        {
            var story = await _context.Blogposts
                .Where(s => s.Id == id)
                .Select(s => new {s.Id, s.Slug, AuthorId = s.Author.Id})
                .AsNoTracking()
                .FirstOrDefaultAsync();
            
            if (story == null) return NotFound();
            if (!User.IsUserSameAsLoggedIn(story.AuthorId)) return RedirectToPage("Index");

            await _blogpostsRepo.TogglePublishedStatus(id);

            return RedirectToPage("./Post", new { id = story.Id, slug = story.Slug });
        }
    }
}