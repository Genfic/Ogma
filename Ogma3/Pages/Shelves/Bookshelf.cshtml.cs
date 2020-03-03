using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Ogma3.Data;
using Ogma3.Data.Models;

namespace Ogma3.Pages.Shelves
{
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
        public List<Story> Stories { get; set; }
        
        public async void OnGetAsync(int id, string? slug)
        {
            Shelf = await _context.Shelves
                .Where(s => s.Id == id)
                .Include(s => s.ShelfStories)
                .FirstAsync();

            // Stories = await _context.Entry(Shelf)
            //     .Collection(s => s.ShelfStories)
            //     .Query()
            //     .Select(s => s.Story)
            //     .Include(s => s.Author)
            //     .Include(s => s.Rating)
            //     .Include(s => s.StoryTags)
            //         .ThenInclude(st => st.Tag)
            //             .ThenInclude(t => t.Namespace)
            //     .ToListAsync();
        }
    }
}