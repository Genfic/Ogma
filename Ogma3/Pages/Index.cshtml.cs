using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Ogma3.Data;
using Ogma3.Data.DTOs;
using Ogma3.Data.Models;

namespace Ogma3.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;
        private readonly ApplicationDbContext _context;

        public List<Story> RecentStories { get; set; }
        public List<Story> TopStories { get; set; }

        public object Counts { get; set; }

        public IndexModel(ApplicationDbContext context, ILogger<IndexModel> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task OnGetAsync()
        {
            RecentStories = await _context.Stories
                .Take(10)
                .OrderByDescending(s => s.ReleaseDate)
                .Include(s => s.StoryTags)
                    .ThenInclude(st => st.Tag)
                        .ThenInclude(t => t.Namespace)
                .Include(s => s.Rating)
                .Include(s => s.Author)
                .AsNoTracking()
                .ToListAsync();
            
            TopStories = await _context.Stories
                // .Where(s => s.ReleaseDate > DateTime.Now - TimeSpan.FromDays(30))
                .OrderByDescending(s => s.Votes.Count)
                    .ThenByDescending(s => s.ReleaseDate)
                .Take(10)
                .Include(s => s.StoryTags)
                    .ThenInclude(st => st.Tag)
                        .ThenInclude(t => t.Namespace)
                .Include(s => s.Rating)
                .Include(s => s.Author)
                .AsNoTracking()
                .ToListAsync();

            Counts = await _context.Users
                .Where(u => u.Id == 7)
                .Select(u => new StoryAndBlogpostCountsDTO
                {
                    Stories = _context.Stories.Count(s => s.Author.Id == u.Id),
                    Blogposts = _context.Blogposts.Count(b => b.Author.Id == u.Id)
                })
                .FirstOrDefaultAsync();
        }
    }
}
