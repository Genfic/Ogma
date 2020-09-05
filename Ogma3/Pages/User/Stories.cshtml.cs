using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Ogma3.Data;
using Ogma3.Data.DTOs;
using Ogma3.Data.Models;

namespace Ogma3.Pages.User
{
    public class StoriesModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public StoriesModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public IList<Story> Stories { get;set; }
        public bool IsCurrentUser { get; set; }

        public OgmaUser Owner { get; set; }
        public StoryAndBlogpostCountsDTO Counts { get; set; }

        public async Task<ActionResult> OnGetAsync(string name)
        {
            Owner = await _context.Users.FirstOrDefaultAsync(u => u.NormalizedUserName == name.ToUpper());
            if (Owner == null) return NotFound();
            IsCurrentUser = Owner.IsLoggedIn(User);
            
            var storiesQuery = IsCurrentUser
                ? _context.Stories.Where(s => s.Author == Owner)
                : _context.Stories.Where(s => s.Author == Owner && s.IsPublished);
            
            Stories = await storiesQuery
                .OrderByDescending(s => s.ReleaseDate)
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

            return Page();
        }
    }
}
