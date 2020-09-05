using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Ogma3.Data;
using Ogma3.Data.DTOs;
using Ogma3.Data.Models;

namespace Ogma3.Pages.User
{
    public class IndexModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        public OgmaUser CurrentUser { get; set; }
        public StoryAndBlogpostCountsDTO Counts { get; set; }

        public IndexModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> OnGetAsync(string name)
        {
            CurrentUser = await _context.Users
                .Where(u => u.NormalizedUserName == name.ToUpper())
                .Include(u => u.CommentsThread)
                .AsNoTracking()
                .FirstOrDefaultAsync();

            if (CurrentUser == null) return NotFound();
            
            Counts = await _context.Users
                .Where(u => u.Id == 7)
                .Select(u => new StoryAndBlogpostCountsDTO
                {
                    Stories = _context.Stories.Count(s => s.Author.Id == u.Id),
                    Blogposts = _context.Blogposts.Count(b => b.Author.Id == u.Id)
                })
                .FirstOrDefaultAsync();
            
            return Page();
        }
    }
}