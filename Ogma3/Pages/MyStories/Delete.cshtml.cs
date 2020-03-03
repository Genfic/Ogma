﻿using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using B2Net;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Ogma3.Data.Models;

namespace Ogma3.Pages.MyStories
{
    [Authorize]
    public class DeleteModel : PageModel
    {
        private readonly Data.ApplicationDbContext _context;
        private IConfiguration _config;
        private readonly IB2Client _b2Client;

        public DeleteModel(Data.ApplicationDbContext context, IB2Client b2Client, IConfiguration config)
        {
            _context = context;
            _b2Client = b2Client;
            _config = config;
        }

        [BindProperty] public Story Story { get; set; }

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

        public async Task<IActionResult> OnPostAsync(int? id)
        {
            if (id == null) return NotFound();

            // Get story
            Story = await _context.Stories
                .Include(s => s.Author)
                .FirstOrDefaultAsync(s => s.Id == id);
            if (Story == null) return RedirectToPage("./Index");

            // Check permissions
            if (Story.Author.Id != User.FindFirstValue(ClaimTypes.NameIdentifier))
                return RedirectToPage("./Index");

            // Remove tag associations
            await _context.StoryTags
                .Where(st => st.StoryId == Story.Id)
                .ForEachAsync(st => _context.StoryTags.Remove(st));

            // Remove story
            _context.Stories.Remove(Story);

            // Delete cover
            if (Story.CoverId != null)
                await _b2Client.Files.Delete(Story.CoverId, Story.Cover.Replace(_config["cdn"], ""));

            // Save
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}