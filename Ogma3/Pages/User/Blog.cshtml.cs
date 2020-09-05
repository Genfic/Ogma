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

namespace Ogma3.Pages.User
{
    public class BlogModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        public IList<Blogpost> Posts { get;set; }
        public int PostsCount { get; set; }
        
        public readonly int PerPage = 25;
        
        public StoryAndBlogpostCountsDTO Counts { get; set; }
        
        public int PageNumber { get; set; }
        public OgmaUser Owner { get; set; }
        public bool IsCurrentUser { get; set; }

        public BlogModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<ActionResult> OnGetAsync(string name, [FromQuery] int page = 1)
        {
            PageNumber = page;
            
            Owner = await _context.Users
                .AsNoTracking()
                .FirstAsync(u => u.NormalizedUserName == name.ToUpper());
            
            if (Owner == null) return NotFound();
            IsCurrentUser = Owner.Id.ToString() == User.FindFirstValue(ClaimTypes.NameIdentifier);

            var postsQuery = IsCurrentUser
                ? _context.Blogposts.Where(b => b.Author == Owner)
                : _context.Blogposts.Where(b => b.Author == Owner && b.IsPublished);

            Posts = await postsQuery
                .Skip(Math.Max(0, page - 1) * PerPage)
                .Take(PerPage)
                .Include(b => b.Author)
                .AsNoTracking()
                .ToListAsync();

            PostsCount = await _context.Blogposts.CountAsync();
            
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
