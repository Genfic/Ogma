using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Ogma3.Data.Models;

namespace Ogma3.Pages.MyStories
{
    [Authorize]
    public class DetailsModel : PageModel
    {
        private readonly Data.ApplicationDbContext _context;

        public DetailsModel(Data.ApplicationDbContext context)
        {
            _context = context;
        }

        public Story Story { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id)
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

            // Check permissions
            if (Story.Author.Id != User.FindFirstValue(ClaimTypes.NameIdentifier))
                return RedirectToPage("./Index");

            if (Story == null) return NotFound();
            
            return Page();
        }
    }
}
