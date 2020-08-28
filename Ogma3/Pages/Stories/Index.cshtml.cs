using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Castle.Core.Internal;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Ogma3.Data;
using Ogma3.Data.Enums;
using Ogma3.Data.Models;

namespace Ogma3.Pages.Stories
{
    public class IndexModel : PageModel
    {
        private ApplicationDbContext _context;

        public List<Rating> Ratings { get; set; }
        public List<Story> Stories { get; set; }
        public int StoriesCount { get; set; }

        public IEnumerable<long> Tags { get; set; }
        
        public int PageNumber { get; set; }
        public readonly int PerPage = 25;
        public EStorySortingOptions SortBy { get; set; }
        public string SearchBy { get; set; }
        public long? Rating { get; set; }
        
        public IndexModel(ApplicationDbContext context)
        {
            _context = context;
        }
        
        public async Task OnGetAsync(
            [FromQuery] IList<long> tags,
            [FromQuery] string search = null, 
            [FromQuery] EStorySortingOptions sort = EStorySortingOptions.DateDescending,
            [FromQuery] long? rating = null,
            [FromQuery] int page = 1
        )
        {
            SearchBy = search;
            SortBy = sort;
            Rating = rating;
            PageNumber = page;
            Tags = tags;
            
            // Load ratings
            Ratings = await _context.Ratings.ToListAsync();
            
            // Prepare search query
            var query = _context.Stories
                .AsQueryable();
            
            // Search by title
            if (!search.IsNullOrEmpty())
            {
                query = query
                    .Where(s => EF.Functions.Like(s.Title.ToUpper(), $"%{search.Trim().ToUpper()}%"));
            }
            
            // Search by rating
            if (rating != null)
            {
                query = query
                    .Where(s => s.Rating.Id == rating);
            }
            
            // Search by tags
            if (tags.Count > 0)
            {
                query = query
                    .Where(s => s.StoryTags.Any(st => tags.Contains(st.TagId)));
            }
            
            // Count stories at this stage
            StoriesCount = await query.CountAsync();
            
            // Sort
            query = sort switch
            {
                EStorySortingOptions.TitleAscending  => query.OrderBy(s => s.Title),
                EStorySortingOptions.TitleDescending => query.OrderByDescending(s => s.Title),
                EStorySortingOptions.WordsAscending  => query.OrderBy(s => s.WordCount),
                EStorySortingOptions.WordsDescending => query.OrderByDescending(s => s.WordCount),
                EStorySortingOptions.DateAscending   => query.OrderBy(s => s.ReleaseDate),
                EStorySortingOptions.DateDescending  => query.OrderByDescending(s => s.ReleaseDate),
                _ => query.OrderByDescending(s => s.ReleaseDate)
            };

            Stories = await query
                .Include(s => s.StoryTags)
                    .ThenInclude(st => st.Tag)
                        .ThenInclude(t => t.Namespace)
                .Include(s => s.Rating)
                .Include(s => s.Author)
                .Skip(Math.Max(0, page - 1) * PerPage)
                .Take(PerPage)
                .AsNoTracking()
                .ToListAsync();
        }

    }
}