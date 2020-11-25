using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Ogma3.Data;
using Ogma3.Data.Models;
using Utils.Extensions;

namespace Ogma3.Pages.Chapters
{
    [Authorize]
    public class CreateModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public CreateModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public class GetModel
        {
            public long? AuthorId { get; set; }
            public string Slug { get; set; }
            public string Title { get; set; }
        }

        public async Task<IActionResult> OnGetAsync(long id)
        {
            Input = new InputModel();

            // Get story
            Story = await _context.Stories
                .Where(s => s.Id == id)
                .Select(s => new GetModel
                {
                    AuthorId = s.AuthorId,
                    Slug = s.Slug,
                    Title = s.Title
                })
                .AsNoTracking()
                .FirstOrDefaultAsync();
            
            // Redirect if story doesn't exist
            if (Story == null) return RedirectToPage("../Index");
            
            // Check ownership, render page if it's ok
            if (Story.AuthorId == User.GetNumericId()) return Page();
            
            // Redirect to the story itself if not an owner
            return RedirectToPage("../Story", new { id, slug = Story.Slug });
        }

        [BindProperty]
        public InputModel Input { get; set; }
        public GetModel Story { get; set; }

        public class InputModel
        {
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

        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync(long id)
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            // Get the story to insert a chapter into. Include user in the search to check ownership.
            var story = await _context.Stories
                .Where(s => s.Id == id)
                .Where(s => s.AuthorId == User.GetNumericId())
                .Include(s => s.Chapters)
                .FirstOrDefaultAsync();

            // Back to index if the story is null or author isn't the logged in user
            if (story == null)
            {
                return RedirectToPage("../Index");
            }

            // Get the order number of the latest chapter
            var latestChapter = story.Chapters
                .OrderByDescending(c => c.Order)
                .Select(c => c.Order)
                .FirstOrDefault();
            
            // Construct new chapter
            var chapter = new Chapter
            {
                Title = Input.Title.Trim(),
                Body = Input.Body.Trim(),
                StartNotes = Input.StartNotes?.Trim(),
                EndNotes = Input.EndNotes?.Trim(),
                Slug = Input.Title.Trim().Friendlify(),
                Order = latestChapter + 1,
                CommentsThread = new CommentsThread(),
                WordCount = Input.Body.Trim().Split(' ', '\t', '\n').Length
            };
            
            // Recalculate words and chapters in the story
            story.WordCount = story.Chapters.Sum(c => c.WordCount) + chapter.WordCount;
            story.ChapterCount = story.Chapters.Count + 1;
            
            // Create the chapter and add it to the story
            story.Chapters.Add(chapter);
            
            await _context.SaveChangesAsync();

            return RedirectToPage("../Chapter", new { id = chapter.Id, slug = chapter.Slug });
        }
    }
}
