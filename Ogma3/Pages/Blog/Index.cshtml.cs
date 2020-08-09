using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Ogma3.Data;
using Ogma3.Data.Models;

namespace Ogma3.Pages.Blog
{
    public class IndexModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        public IList<Blogpost> Posts { get;set; }
        public int PostsCount { get; set; }
        
        public readonly int PerPage = 25;
        
        public int Page { get; set; }
        public User Owner { get; set; }
        public bool IsCurrentUser { get; set; }

        public IndexModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<ActionResult> OnGetAsync(string name, [FromQuery] int page = 1)
        {
            Page = page;
            
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

            return Page();
        }
    }
}
