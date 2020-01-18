using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Ogma3.Data.Models;

namespace Ogma3.Pages
{
    [Authorize]
    public class StoryModel : PageModel
    {
        private readonly Data.ApplicationDbContext _context;

        public StoryModel(Data.ApplicationDbContext context)
        {
            _context = context;
        }

        public Story Story { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id, string? slug)
        {
            if (id == null) return NotFound();

            Story = await _context.Stories
                .Include(s => s.Author)
                .Include(s => s.StoryTags)
                    .ThenInclude(st => st.Tag)
                        .ThenInclude(t => t.Namespace)
                .Include(s => s.Rating)
                .Include(s => s.Chapters)
                .FirstOrDefaultAsync(s => s.Id == id);

            if (Story == null) return NotFound();
            
            return Page();
        }
    }
}
