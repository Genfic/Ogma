using System.Linq;
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
    public class DeleteModel : PageModel
    {
        private readonly Data.ApplicationDbContext _context;

        public DeleteModel(Data.ApplicationDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Story Story { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null) return NotFound();

            Story = await _context.Stories
                .Include(s => s.Author)
                .FirstOrDefaultAsync(s => s.Id == id);

            // Check permissions
            if (Story.Author.Id != User.FindFirstValue(ClaimTypes.NameIdentifier))
                return RedirectToPage("./Index");

            if (Story == null) return NotFound();
            
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int? id)
        {
            if (id == null) return NotFound();
            
            Story = await _context.Stories
                .Include(s => s.Author)
                .FirstOrDefaultAsync(s => s.Id == id);
            if (Story == null) return RedirectToPage("./Index");
            
            // Check permissions
            if (Story.Author.Id != User.FindFirstValue(ClaimTypes.NameIdentifier))
                return RedirectToPage("./Index");

            await _context.StoryTags
                .Where(st => st.StoryId == Story.Id)
                .ForEachAsync(st => _context.StoryTags.Remove(st));
            
            _context.Stories.Remove(Story);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}
