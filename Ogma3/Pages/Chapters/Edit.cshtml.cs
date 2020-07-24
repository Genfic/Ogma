using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Ogma3.Data;
using Utils.Extensions;

namespace Ogma3.Pages.Chapters
{
    [Authorize]
    public class EditModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private OgmaUserManager _userManager;

        public EditModel(ApplicationDbContext context, OgmaUserManager userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        [BindProperty]
        public InputModel Input { get; set; }
        
        public class InputModel
        {
            public long Id { get; set; }
            
            [Required]
            [StringLength(
                CTConfig.CChapter.MaxTitleLength,
                ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", 
                MinimumLength = CTConfig.CChapter.MinTitleLength
            )]
            public string Title { get; set; }
            
            [Required]
            [StringLength(
                CTConfig.CChapter.MaxBodyLength,
                ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", 
                MinimumLength = CTConfig.CChapter.MinBodyLength
            )]
            public string Body { get; set; }
            
            [StringLength(
                CTConfig.CChapter.MaxNotesLength,
                ErrorMessage = "The {0} cannot exceed {1} characters."
            )]
            public string StartNotes { get; set; }
            
            [StringLength(
                CTConfig.CChapter.MaxNotesLength,
                ErrorMessage = "The {0} cannot exceed {1} characters."
            )]
            public string EndNotes { get; set; }
        }

        public async Task<IActionResult> OnGetAsync(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            // Get chapter
            var chapter = await _context.Chapters
                .Where(c => c.Id == id)
                .AsNoTracking()
                .FirstOrDefaultAsync();
            // Get logged in user
            var user = await _userManager.GetUserAsync(User);
            // Make sure the story's author is the logged in user
            var authorized = await _context.Stories
                .AsNoTracking()
                .AnyAsync(s => s.Id == chapter.StoryId && s.Author == user);

            if (chapter == null || !authorized) return NotFound();

            Input = new InputModel
            {
                Id = chapter.Id,
                Title = chapter.Title,
                Body = chapter.Body,
                StartNotes = chapter.StartNotes,
                EndNotes = chapter.EndNotes
            };
            
            return Page();
        }

        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }
            
            // Get chapter
            var chapter = await _context.Chapters.FindAsync(Input.Id);
            if (chapter == null) return NotFound();
            
            // Get story
            var story = await _context.Stories
                .Where(s => s.Id == chapter.StoryId)
                .Include(s => s.Chapters)
                .Include(s => s.Author)
                .FirstOrDefaultAsync();
            if (story == null) return NotFound();
            
            if (!story.Author.IsLoggedIn(User)) return NotFound();
            
            chapter.Title      = Input.Title.Trim();
            chapter.Body       = Input.Body.Trim();
            chapter.StartNotes = Input.StartNotes?.Trim();
            chapter.EndNotes   = Input.EndNotes?.Trim();
            chapter.Slug       = Input.Title.Trim().Friendlify();
            chapter.WordCount  = Input.Body.Trim().Split(' ', '\t', '\n').Length;
            
            await _context.SaveChangesAsync();

            // Recalculate words in the story
            story.WordCount = story.Chapters.Sum(c => c.WordCount);

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ChapterExists(Input.Id))
                {
                    return NotFound();
                }
                throw;
            }

            return RedirectToPage("../Chapter", new { id = chapter.Id, slug = chapter.Slug });
        }

        private bool ChapterExists(long id)
        {
            return _context.Chapters.Any(e => e.Id == id);
        }
    }
}
