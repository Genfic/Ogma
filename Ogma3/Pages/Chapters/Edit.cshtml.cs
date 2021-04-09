using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Ogma3.Data;
using Ogma3.Infrastructure.Extensions;
using Utils.Extensions;

namespace Ogma3.Pages.Chapters
{
    [Authorize]
    public class EditModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public EditModel(ApplicationDbContext context)
        {
            _context = context;
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
            [Display(Name = "Start notes")]
            public string StartNotes { get; set; }
            
            [StringLength(
                CTConfig.CChapter.MaxNotesLength,
                ErrorMessage = "The {0} cannot exceed {1} characters."
            )]
            [Display(Name = "End notes")]
            public string EndNotes { get; set; }

            [Required]
            public bool Published { get; set; }
        }

        public async Task<IActionResult> OnGetAsync(long id)
        {
            // Get chapter
            var chapter = await _context.Chapters
                .Where(c => c.Id == id)
                .AsNoTracking()
                .FirstOrDefaultAsync();
            
            if (chapter == null) return NotFound();
            
            // Make sure the story's author is the logged in user
            var authorized = await _context.Stories
                .Where(s => s.Id == chapter.StoryId)
                .Where(s => s.AuthorId == User.GetNumericId())
                .AsNoTracking()
                .AnyAsync();

            if (!authorized) return Unauthorized();

            Input = new InputModel
            {
                Id = chapter.Id,
                Title = chapter.Title,
                Body = chapter.Body,
                StartNotes = chapter.StartNotes,
                EndNotes = chapter.EndNotes,
                Published = chapter.IsPublished
            };
            
            return Page();
        }

        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync(long id)
        {
            if (!ModelState.IsValid) return Page();
            
            // Get chapter
            var chapter = await _context.Chapters.FindAsync(id);
            if (chapter == null) return NotFound();
            
            // Get story
            var story = await _context.Stories
                .Where(s => s.Id == chapter.StoryId)
                .Where(s => s.AuthorId == User.GetNumericId())
                .Include(s => s.Chapters)
                .FirstOrDefaultAsync();
            if (story == null) return NotFound();
            
            // Update the chapter
            chapter.Title       = Input.Title.Trim();
            chapter.Body        = Input.Body.Trim();
            chapter.StartNotes  = Input.StartNotes?.Trim();
            chapter.EndNotes    = Input.EndNotes?.Trim();
            chapter.Slug        = Input.Title.Trim().Friendlify();
            chapter.WordCount   = Input.Body.Trim().Split(' ', '\t', '\n').Length;
            chapter.IsPublished = Input.Published;
            await _context.SaveChangesAsync();

            // Recalculate words in the story
            story.WordCount = story.Chapters.Sum(c => c.WordCount);
            // Recalculate chapters in the story
            story.ChapterCount = story.Chapters.Count(c => c.IsPublished);
            await _context.SaveChangesAsync();
            
            // Check if story has any published chapter, and if not, unpublish it
            if (!story.Chapters.Any(c => c.IsPublished))
            {
                story.IsPublished = false;
                await _context.SaveChangesAsync();
            }
            
            return RedirectToPage("../Chapter", new { id = chapter.Id, slug = chapter.Slug });
        }
    }
}
