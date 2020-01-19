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
            
            Console.WriteLine($"ID is {id}");
            
            _context.Chapters.Add(Chapter);

            Story = _context.Stories
                .Where(s => s.Id == id)
                .Include(s => s.Chapters)
                .First();
            Story.Chapters.Add(Chapter);
            
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}
