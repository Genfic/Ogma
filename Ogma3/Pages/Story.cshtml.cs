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
using Ogma3.Data.Users;
using Ogma3.Infrastructure.Extensions;
using Ogma3.Pages.Shared.Bars;

namespace Ogma3.Pages
{
    
    public class StoryModel : PageModel
    {
        private readonly UserRepository _userRepo;
        private readonly ApplicationDbContext _context;

        public StoryModel(UserRepository userRepo, ApplicationDbContext context)
        {
            _userRepo = userRepo;
            _context = context;
        }

        public class StoryDetails
        {
            public long Id { get; init; }
            public long AuthorId { get; init; }
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
            public int ChaptersCount { get; init; }
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
        public ProfileBar ProfileBar { get; private set; }

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
                    ChaptersCount = s.ChapterCount,
                    CommentsCount = s.Chapters.Sum(c => c.CommentsThread.CommentsCount),
                    IsPublished = s.IsPublished,
                    ReleaseDate = s.ReleaseDate,
                    WordCount = s.WordCount,
                    ContentBlockId = s.ContentBlockId,
                    Tags = s.Tags
                        .OrderByDescending(t => t.Namespace.HasValue)
                        .ThenByDescending(t => t.Namespace)
                        .Select(t => new TagDto
                        {
                            Id = t.Id,
                            Name = t.Name,
                            Slug = t.Slug,
                            Namespace = t.Namespace,
                            Description = t.Description
                        })
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
            
            ProfileBar = await _userRepo.GetProfileBar(Story.AuthorId);
            
            return Page();
        }
    }
}
