﻿using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Ogma3.Data;
using Ogma3.Data.Stories;
using Ogma3.Infrastructure.Extensions;
using Ogma3.Services.FileUploader;

namespace Ogma3.Pages.Stories;

[Authorize]
public class DeleteModel : PageModel
{
    private readonly ApplicationDbContext _context;
    private readonly ImageUploader _uploader;
        
    public DeleteModel(ApplicationDbContext context, ImageUploader uploader)
    {
        _context = context;
        _uploader = uploader;
    }
        
    public class GetData
    {
        public long Id { get; init; }
        public string Title { get; init; }
        public string Slug { get; init; }
        public DateTime? ReleaseDate { get; init; }
        public DateTime CreationDate { get; set; }
        public string Hook { get; init; }
        public bool IsPublished { get; init; }
        public EStoryStatus Status { get; init; }
        public int VotesCount { get; init; }
        public int ChaptersCount { get; init; }
        public int CommentsCount { get; init; }
    }

    [BindProperty] 
    public GetData Story { get; set; }

    public async Task<IActionResult> OnGetAsync(int? id)
    {
        if (id is null) return NotFound();
        var uid = User.GetNumericId();

        // Get the story and make sure the logged-in user matches author
        Story = await _context.Stories
            .Where(s => s.Id == id)
            .Where(s => s.AuthorId == uid)
            .Select(s => new GetData
            {
                Id = s.Id,
                Title = s.Title,
                Slug = s.Slug,
                ReleaseDate = s.PublicationDate,
                CreationDate = s.CreationDate,
                Hook = s.Hook,
                IsPublished = s.PublicationDate != null,
                Status = s.Status,
                VotesCount = s.Votes.Count,
                ChaptersCount = s.ChapterCount,
                CommentsCount = s.Chapters.Sum(c => c.CommentsThread.CommentsCount)
            })
            .AsNoTracking()
            .FirstOrDefaultAsync();

        if (Story is null) return NotFound();

        return Page();
    }

    public async Task<IActionResult> OnPostAsync(int? id)
    {
        if (id is null) return NotFound();
        var uid = User.GetNumericId();
            
        // Get the story and make sure the logged-in user matches author
        var story = await _context.Stories
            .Where(s => s.Id == id)
            .FirstOrDefaultAsync();
            
        if (story is null) return NotFound();
        if (story.AuthorId != uid) return Unauthorized();

        // Remove story
        _context.Stories.Remove(story);
            
        // Delete cover
        if (story.CoverId is not null && story.Cover is not null)
        {
            await _uploader.Delete(story.Cover, story.CoverId);
        }

        // Save
        await _context.SaveChangesAsync();

        return RedirectToPage("/User/Stories", new { Name = User.GetUsername() });
    }
}