using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Ogma3.Data;
using Ogma3.Data.Models;

namespace Ogma3.Pages.Blog
{
    public class DetailsModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public DetailsModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public Blogpost Blogpost { get; set; }

        public async Task<IActionResult> OnGetAsync(int id, string? slug)
        {
            Blogpost = await _context.Blogposts
                .Where(b => b.Id == id)
                .Include(b => b.Author)
                .Include(b => b.CommentsThread)
                .FirstOrDefaultAsync();

            if (Blogpost == null)
            {
                return NotFound();
            }
            return Page();
        }
    }
}
