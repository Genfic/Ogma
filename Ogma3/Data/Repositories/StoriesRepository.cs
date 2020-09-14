using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Ogma3.Data.DTOs;
using Ogma3.Data.Enums;
using Ogma3.Pages.Shared;

namespace Ogma3.Data.Repositories
{
    public class StoriesRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly ClaimsPrincipal _claims;

        public StoriesRepository(ApplicationDbContext context, ClaimsPrincipal claims)
        {
            _context = context;
            _claims = claims;
        }

        public async Task<StoryDetails> GetStoryDetails(long id)
        {
            return await _context.Stories
                .Where(s => s.Id == id)
                .Select(s => new StoryDetails
                {
                    Id = s.Id,
                    AuthorId = s.Author.Id,
                    IsAuthor = s.Author.Id.ToString() == _claims.FindFirstValue(ClaimTypes.NameIdentifier),
                    Title = s.Title,
                    Slug = s.Slug,
                    Description = s.Description,
                    Hook = s.Hook,
                    Cover = s.Cover,
                    ReleaseDate = s.ReleaseDate,
                    IsPublished = s.IsPublished,
                    Chapters = s.Chapters,
                    Tags = s.StoryTags.Select(st => new TagDTO
                    {
                        Id = st.Tag.Id,
                        Name = st.Tag.Name,
                        Slug = st.Tag.Slug,
                        Description = st.Tag.Description,
                        Namespace = st.Tag.Namespace.Name,
                        Color = st.Tag.Namespace.Color
                    }).ToList(),
                    Rating = s.Rating,
                    Status = s.Status,
                    WordCount = s.WordCount,
                    ChapterCount = s.ChapterCount,
                    Score = s.Votes.Count
                })
                .AsNoTracking()
                .FirstOrDefaultAsync();
        }

        public async Task<List<StoryCard>> GetPaginatedStoryCards(int perPage = 10, int page = 1, EStorySortingOptions order = EStorySortingOptions.DateDescending)
        {
            var query = _context.Stories.AsQueryable();

            query = order switch
            {
                EStorySortingOptions.TitleAscending => query.OrderBy(s => s.Title),
                EStorySortingOptions.TitleDescending => query.OrderByDescending(s => s.Title),
                EStorySortingOptions.DateAscending => query.OrderBy(s => s.ReleaseDate),
                EStorySortingOptions.DateDescending => query.OrderByDescending(s => s.ReleaseDate),
                EStorySortingOptions.WordsAscending => query.OrderBy(s => s.WordCount),
                EStorySortingOptions.WordsDescending => query.OrderByDescending(s => s.WordCount),
                EStorySortingOptions.ScoreAscending => query.OrderBy(s => s.Votes.Count),
                EStorySortingOptions.ScoreDescending => query.OrderByDescending(s => s.Votes.Count),
                _ => query.OrderByDescending(s => s.ReleaseDate)
            };

            return await query
                .Take(perPage)
                .Skip(Math.Max(0, page - 1) * perPage)
                .Select(s => new StoryCard
                {
                    Id = s.Id,
                    AuthorName = s.Author.UserName,
                    Title = s.Title,
                    Slug = s.Slug,
                    Hook = s.Hook,
                    Cover = s.Cover,
                    ReleaseDate = s.ReleaseDate,
                    Tags = s.StoryTags.Select(st => new TagDTO
                    {
                        Id = st.Tag.Id,
                        Name = st.Tag.Name,
                        Slug = st.Tag.Slug,
                        Description = st.Tag.Description,
                        Namespace = st.Tag.Namespace.Name,
                        Color = st.Tag.Namespace.Color
                    }).ToList(),
                    Rating = s.Rating,
                    Status = s.Status,
                    WordCount = s.WordCount,
                    ChapterCount = s.ChapterCount,
                })
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<int> CountForUser(long id)
        {
            return await _context.Stories
                .Where(s => s.Author.Id == id)
                .CountAsync();
        }
    }
}