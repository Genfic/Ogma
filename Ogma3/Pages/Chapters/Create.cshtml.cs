using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Ogma3.Data;
using Ogma3.Data.Models;

namespace Ogma3.Pages.Chapters
{
    public class CreateModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public CreateModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult OnGetAsync(int? id)
        {
            // Get story
            Story = _context.Stories
                .Where(s => s.Id == id)
                .Include(s => s.StoryTags)
                .Include(s => s.Rating)
                .Include(s => s.Author)
                .First();
            
            // Redirect if story doesn't exist
            if (Story == null) return RedirectToPage("../Index");
            
            // Check ownership, render page if it's ok
            if (Story.Author.Id == User.FindFirstValue(ClaimTypes.NameIdentifier))
                return Page();
            
            // Redirect to the story itself if not an owner
            return RedirectToPage("../Story", new { id, slug = Story.Slug });
        }

        [BindProperty]
        public Chapter Chapter { get; set; }
        public Story Story { get; set; }

        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync(int id)
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            // Get the story to insert a chapter into
            Story = _context.Stories
                .Where(s => s.Id == id)
                .Include(s => s.Chapters)
                .First();
            
            // Get the order number of the latest chapter
            var latestChapter = Story.Chapters
                .OrderBy(c => c.Order)
                .LastOrDefault()
                .Order;
            
            // Set this number
            Chapter.Order = latestChapter + 1;
            
            // Create the chapter and add it to the story
            _context.Chapters.Add(Chapter);
            Story.Chapters.Add(Chapter);
            
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}
