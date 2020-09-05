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

namespace Ogma3.Pages.User
{
    public class BlogModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly ProfileBarRepository _profileBarRepo;
        
        public IList<Blogpost> Posts { get;set; }
        
        public readonly int PerPage = 25;
        
        
        public int PageNumber { get; set; }
        public ProfileBarDTO ProfileBar { get; set; }
        public bool IsCurrentUser { get; set; }

        public BlogModel(ApplicationDbContext context, ProfileBarRepository profileBarRepo)
        {
            _context = context;
            _profileBarRepo = profileBarRepo;
        }

        public async Task<ActionResult> OnGetAsync(string name, [FromQuery] int page = 1)
        {
            PageNumber = page;
            
            ProfileBar = await _profileBarRepo.GetAsync(name.ToUpper());
            if (ProfileBar == null) return NotFound();

            IsCurrentUser = ProfileBar.Id.ToString() == User.FindFirstValue(ClaimTypes.NameIdentifier);

            var postsQuery = IsCurrentUser
                ? _context.Blogposts.Where(b => b.Author.Id == ProfileBar.Id)
                : _context.Blogposts.Where(b => b.Author.Id == ProfileBar.Id && b.IsPublished);

            Posts = await postsQuery
                .Skip(Math.Max(0, page - 1) * PerPage)
                .Take(PerPage)
                .Include(b => b.Author)
                .AsNoTracking()
                .ToListAsync();
            
            return Page();
        }

    }
}
