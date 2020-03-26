using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Ogma3.Data;
using Ogma3.Data.Models;

namespace Ogma3.Pages.Stories
{
    [Authorize]
    public class IndexModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public IndexModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public IList<Story> Stories { get;set; }
        public bool IsCurrentUser { get; set; }

        public User Owner { get; set; }

        public async Task<ActionResult> OnGetAsync(string name)
        {
            Owner = await _context.Users.FirstOrDefaultAsync(u => u.NormalizedUserName == name.ToUpper());
            if (Owner == null) return NotFound();
            IsCurrentUser = Owner.Id == User.FindFirstValue(ClaimTypes.NameIdentifier);
            
            var storiesQuery = IsCurrentUser
                ? _context.Stories.Where(s => s.Author == Owner)
                : _context.Stories.Where(s => s.Author == Owner && s.IsPublished);
            
            Stories = await storiesQuery
                .Include(s => s.StoryTags)
                    .ThenInclude(st => st.Tag)
                        .ThenInclude(t => t.Namespace)
                .Include(s => s.Rating)
                .Include(s => s.Author)
                .ToListAsync();

            return Page();
        }
    }
}
