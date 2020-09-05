using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Ogma3.Data;
using Ogma3.Data.DTOs;
using Ogma3.Data.Models;
using Ogma3.Data.Repositories;

namespace Ogma3.Pages.User
{
    public class StoriesModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly ProfileBarRepository _profileBarRepo;

        public StoriesModel(ApplicationDbContext context, ProfileBarRepository profileBarRepo)
        {
            _context = context;
            _profileBarRepo = profileBarRepo;
        }

        public IList<Story> Stories { get;set; }
        
        public readonly int PerPage = 25;

        public int PageNumber { get; set; }
        
        public ProfileBarDTO ProfileBar { get; set; }
        public bool IsCurrentUser { get; set; }

        public async Task<ActionResult> OnGetAsync(string name, [FromQuery] int page = 1)
        {
            PageNumber = page;
            
            ProfileBar = await _profileBarRepo.GetAsync(name.ToUpper());
            if (ProfileBar == null) return NotFound();

            IsCurrentUser = ProfileBar.Id.ToString() == User.FindFirstValue(ClaimTypes.NameIdentifier);
            
            var storiesQuery = IsCurrentUser
                ? _context.Stories.Where(s => s.Author.Id == ProfileBar.Id)
                : _context.Stories.Where(s => s.Author.Id == ProfileBar.Id && s.IsPublished);
            
            Stories = await storiesQuery
                .OrderByDescending(s => s.ReleaseDate)
                .Include(s => s.StoryTags)
                    .ThenInclude(st => st.Tag)
                        .ThenInclude(t => t.Namespace)
                .Include(s => s.Rating)
                .Include(s => s.Author)
                .AsNoTracking()
                .ToListAsync();

            return Page();
        }
    }
}
