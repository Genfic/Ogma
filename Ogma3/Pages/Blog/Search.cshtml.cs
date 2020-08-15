using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Ogma3.Data;
using Ogma3.Data.Models;

namespace Ogma3.Pages.Blog
{
    public class SearchModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        public IList<Blogpost> Posts { get;set; }
        public int PostsCount { get; set; }
        
        public readonly int PerPage = 25;
        
        public int PageNumber { get; set; }

        public SearchModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<ActionResult> OnGetAsync([FromQuery] string t = null, [FromQuery] string q = null, [FromQuery] int page = 1)
        {
            PageNumber = page;

            var query = _context.Blogposts.AsQueryable();
            
            if (t != null) // Query by tags if they're in the search
            {
                var tags = t.Split(',').Select(tag => '#' + tag.TrimStart('#')).ToArray();
                query = query.Where(b => tags.All(i => b.Hashtags.Contains(i)));
            }
            
            if (q != null) // query by title if it's in the search
            {
                query = query.Where(b => EF.Functions.Like(b.Title.ToUpper(), $"%{q}%".ToUpper()));
            }
            
            Posts = await query
                .Include(b => b.Author)
                .Skip(Math.Max(0, page - 1) * PerPage)
                .Take(PerPage)
                .AsNoTracking()
                .ToListAsync();
            
            PostsCount = await _context.Blogposts.CountAsync();
            
            return Page();
        }
    }
}