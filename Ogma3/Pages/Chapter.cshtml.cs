using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Ogma3.Data;
using Ogma3.Data.DTOs;

namespace Ogma3.Pages
{
    public class ChapterModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public ChapterModel(ApplicationDbContext context)
        {
            _context = context;
        }
        
        public StoryChapterDTO StoryChapter { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var chapter = await _context.Chapters.FirstOrDefaultAsync(m => m.Id == id);
            var story = await _context.Stories.FirstOrDefaultAsync(s => s.Id == chapter.StoryId);
            StoryChapter = new StoryChapterDTO
            {
                Chapter = chapter,
                Story = story
            };

            if (chapter == null)
            {
                return NotFound();
            }
            return Page();
        }
    }
}
