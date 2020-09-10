using System;
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
using Ogma3.Pages.Shared;

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

        private const int PerPage = 25;
        public ProfileBarDTO ProfileBar { get; set; }
        public bool IsCurrentUser { get; set; }
        public PaginationModel PaginationModel { get; set; }

        public async Task<ActionResult> OnGetAsync(string name, [FromQuery] int page = 1)
        {
            ProfileBar = await _profileBarRepo.GetAsync(name.ToUpper());
            if (ProfileBar == null) return NotFound();

            IsCurrentUser = ProfileBar.Id.ToString() == User.FindFirstValue(ClaimTypes.NameIdentifier);
            
            var query = IsCurrentUser
                ? _context.Stories.Where(s => s.Author.Id == ProfileBar.Id)
                : _context.Stories.Where(s => s.Author.Id == ProfileBar.Id && s.IsPublished);

            var storiesCount = await query.CountAsync();
            
            Stories = await query
                .OrderByDescending(s => s.ReleaseDate)
                .Skip(Math.Max(0, page - 1) * PerPage)
                .Take(PerPage)
                .Include(s => s.StoryTags)
                    .ThenInclude(st => st.Tag)
                        .ThenInclude(t => t.Namespace)
                .Include(s => s.Rating)
                .Include(s => s.Author)
                .AsNoTracking()
                .ToListAsync();

            // Prepare pagination
            PaginationModel = new PaginationModel
            {
                CurrentPage = page,
                ItemCount = storiesCount,
                PerPage = PerPage
            };
            
            return Page();
        }
    }
}
