using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Ogma3.Data;
using Ogma3.Data.Ratings;
using Ogma3.Data.Stories;
using Ogma3.Data.Tags;
using Ogma3.Infrastructure.Extensions;

namespace Ogma3.Pages
{
    
    public class StoryModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public StoryModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public class StoryDetails
        {
            public long Id { get; init; }
            public long AuthorId { get; init; }
            public string AuthorName { get; init; }
            public string Title { get; init; }
            public string Slug { get; init; }
            public string Description { get; init; }
            public string Hook { get; init; }
            public string? Cover { get; init; }
            public DateTime ReleaseDate { get; init; }
            public bool IsPublished { get; init; }
            public  ICollection<ChapterBasicDto> Chapters { get; init; }
            public ICollection<TagDto> Tags { get; init; }
            public Rating Rating { get; init; }
            public EStoryStatus Status { get; init; }
            public int WordCount { get; init; }
            public int FullWordCount { get; init; }
            public int ChaptersCount { get; init; }
            public int FullChaptersCount { get; init; }
            public int CommentsCount { get; init; }
            public int Score { get; init; }
            public long? ContentBlockId { get; init; }
        }
        
        public class ChapterBasicDto
        {
            public long Id { get; init; }
            public uint Order { get; init; }
            public string Slug { get; init; }
            public string Title { get; init; }
            public DateTime PublishDate { get; init; }
            public int WordCount { get; init; }
        }
        
        public StoryDetails Story { get; private set; }

        public async Task<IActionResult> OnGetAsync(long id, string? slug)
        {
            var uid = User.GetNumericId();
            
            Story = await _context.Stories
                .Where(s => s.Id == id)
                .Where(s => s.IsPublished || s.AuthorId == uid)
                .Select(s => new StoryDetails
                {
                    Id = s.Id,
                    Title = s.Title,
                    Slug = s.Slug,
                    Hook = s.Hook,
                    Description = s.Description,
                    Cover = s.Cover,
                    Rating = s.Rating,
                    Score = s.Votes.Count,
                    Status = s.Status,
                    AuthorId = s.AuthorId,
                    AuthorName = s.Author.NormalizedUserName,
                    WordCount = s.WordCount,
                    FullWordCount = s.AuthorId == uid ? s.Chapters.Sum(c => c.WordCount) : 0,
                    ChaptersCount = s.ChapterCount,
                    FullChaptersCount = s.AuthorId == uid ? s.Chapters.Count : 0,
                    CommentsCount = s.Chapters.Sum(c => c.CommentsThread.CommentsCount),
                    IsPublished = s.IsPublished,
                    ReleaseDate = s.ReleaseDate,
                    ContentBlockId = s.ContentBlockId,
                    Tags = s.Tags.AsQueryable()
                        .OrderByDescending(t => t.Namespace.HasValue)
                        .ThenByDescending(t => t.Namespace)
                        .Select(TagMappings.ToTagDto)
                        .ToList(),
                    Chapters = s.Chapters
                        .Where(c => c.IsPublished || c.Story.AuthorId == uid)
                        .Where(c => c.ContentBlockId == null || c.Story.AuthorId == uid)
                        .Select(c => new ChapterBasicDto
                        {
                            Id = c.Id,
                            Title = c.Title,
                            Slug = c.Slug,
                            PublishDate = c.PublishDate,
                            Order = c.Order,
                            WordCount = c.WordCount
                        })
                        .ToList()
                })
                .AsNoTracking()
                .FirstOrDefaultAsync();

            if (Story == null) return NotFound();
            
            return Page();
        }
    }
}
