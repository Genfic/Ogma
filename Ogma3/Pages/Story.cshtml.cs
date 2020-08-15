using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Ogma3.Data.Models;

namespace Ogma3.Pages
{
    
    public class StoryModel : PageModel
    {
        private readonly Data.ApplicationDbContext _context;

        public StoryModel(Data.ApplicationDbContext context)
        {
            _context = context;
        }

        public Story Story { get; set; }

        public async Task<IActionResult> OnGetAsync(long id, string? slug)
        {
            Story = await _context.Stories
                .Include(s => s.Author)
                .Include(s => s.StoryTags)
                    .ThenInclude(st => st.Tag)
                        .ThenInclude(t => t.Namespace)
                .Include(s => s.Rating)
                .Include(s => s.Chapters)
                .Include(s => s.Votes)
                .AsNoTracking()
                .FirstOrDefaultAsync(s => s.Id == id);

            if (Story == null) return NotFound();
            
            return Page();
        }
    }
}
