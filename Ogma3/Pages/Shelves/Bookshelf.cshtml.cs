using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Ogma3.Data;
using Ogma3.Data.Models;

namespace Ogma3.Pages.Shelves
{
    [Authorize]
    public class Bookshelf : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<User> _userManager;

        public Bookshelf(ApplicationDbContext context, UserManager<User> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public Shelf Shelf { get; set; }
        
        public async Task<IActionResult> OnGetAsync(int? id, string? slug)
        {
            if (id == null) return NotFound();

            var user = await _userManager.GetUserAsync(User);
            
            Shelf = await _context.Shelves
                .Where(s => s.Id == id && s.Owner == user)
                .Include(s => s.ShelfStories)
                    .ThenInclude(ss => ss.Story)
                        .ThenInclude(s => s.StoryTags)
                            .ThenInclude(st => st.Tag)
                                .ThenInclude(t => t.Namespace)
                .Include(s => s.ShelfStories)
                    .ThenInclude(ss => ss.Story)
                        .ThenInclude(s => s.Author)
                .Include(s => s.ShelfStories)
                    .ThenInclude(ss => ss.Story)
                        .ThenInclude(s => s.Rating)
                .AsNoTracking()
                .FirstOrDefaultAsync();
            
            if (Shelf == null) return NotFound();

            return Page();
        }
    }
}