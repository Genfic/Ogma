using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Ogma3.Data;
using Ogma3.Data.Repositories;
using Ogma3.Pages.Shared;
using Ogma3.Pages.Shared.Details;
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
            Blogpost = await _blogpostsRepo.GetDetails(id);

            if (Blogpost == null) return NotFound();
            if (!User.IsUserSameAsLoggedIn(Blogpost.AuthorId)) return RedirectToPage("Index");

            return Page();
        }

        public async Task<IActionResult> OnPostAsync(long id)
        {
            var blog = await _context.Blogposts
                .Where(s => s.Id == id)
                .FirstOrDefaultAsync();
            
            if (blog == null) return NotFound();
            if (!User.IsUserSameAsLoggedIn(blog.AuthorId)) return RedirectToPage("Index");

            blog.IsPublished = !blog.IsPublished;
            await _context.SaveChangesAsync();

            return RedirectToPage("./Post", new { id = blog.Id, slug = blog.Slug });
        }
    }
}