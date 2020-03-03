﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Ogma3.Data;
using Ogma3.Data.Models;
using Utils;

namespace Ogma3.Pages.Chapters
{
    public class CreateModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<User> _userManager;

        public CreateModel(ApplicationDbContext context, UserManager<User> userManager)
        {
            _context = context;
            _userManager = userManager;
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
        public InputModel Chapter { get; set; }
        public Story Story { get; set; }

        public class InputModel
        {
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

        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync(int id)
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            // Get logged in user
            var user = await _userManager.GetUserAsync(User);

            // Get the story to insert a chapter into. Include user in the search to check ownership.
            Story = _context.Stories.First(s => s.Id == id && s.Author.Id == user.Id);

            // Back to index if the story is null
            if (Story == null)
            {
                return RedirectToPage("../Index");
            }

            // Get the order number of the latest chapter
            var latestChapter = await _context.Chapters
                .Where(c => c.StoryId == Story.Id)
                .OrderBy(c => c.Order)
                .Select(c => c.Order)
                .LastOrDefaultAsync();

            // Construct new chapter
            var chapter = new Chapter
            {
                Title = Chapter.Title.Trim(),
                Body = Chapter.Body.Trim(),
                StartNotes = Chapter.StartNotes?.Trim(),
                EndNotes = Chapter.EndNotes?.Trim(),
                Slug = Chapter.Title.Trim().Friendlify(),
                Order = latestChapter + 1
            };
            
            
            // Create the chapter and add it to the story
            _context.Chapters.Add(chapter);
            Story.Chapters.Add(chapter);
            
            await _context.SaveChangesAsync();

            return RedirectToPage("../Chapter", new { id = chapter.Id, slug = chapter.Slug });
        }
    }
}