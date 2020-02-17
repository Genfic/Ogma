using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Ogma3.Data;
using Ogma3.Data.Models;
using Utils;

namespace Ogma3.Pages.Chapters
{
    public class EditModel : PageModel
    {
        private readonly Ogma3.Data.ApplicationDbContext _context;

        public EditModel(Ogma3.Data.ApplicationDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public InputModel Chapter { get; set; }
        
        public class InputModel
        {
            public int Id { get; set; }
            
            [Required]
            [StringLength(
                CTConfig.Chapter.MaxTitleLength,
                ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", 
                MinimumLength = CTConfig.Chapter.MinTitleLength
            )]
            public string Title { get; set; }
            
            [Required]
            [StringLength(
                CTConfig.Chapter.MaxBodyLength,
                ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", 
                MinimumLength = CTConfig.Chapter.MinBodyLength
            )]
            public string Body { get; set; }
            
            [StringLength(
                CTConfig.Chapter.MaxNotesLength,
                ErrorMessage = "The {0} cannot exceed {1} characters."
            )]
            public string StartNotes { get; set; }
            
            [StringLength(
                CTConfig.Chapter.MaxNotesLength,
                ErrorMessage = "The {0} cannot exceed {1} characters."
            )]
            public string EndNotes { get; set; }
        }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var chapter = await _context.Chapters.FirstOrDefaultAsync(m => m.Id == id);

            if (chapter == null)
            {
                return NotFound();
            }
            
            Chapter = new InputModel
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
            
            var chapter = await _context.Chapters.FirstAsync(c => c.Id == Chapter.Id);
            chapter.Title = Chapter.Title.Trim();
            chapter.Body = Chapter.Body.Trim();
            chapter.StartNotes = Chapter.StartNotes?.Trim();
            chapter.EndNotes = Chapter.EndNotes?.Trim();
            chapter.Slug = Chapter.Title.Trim().Friendlify();

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ChapterExists(Chapter.Id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return RedirectToPage("../Chapter", new { id = chapter.Id, slug = chapter.Slug });
        }

        private bool ChapterExists(int id)
        {
            return _context.Chapters.Any(e => e.Id == id);
        }
    }
}
