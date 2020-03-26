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

        public User RequestedUser { get; set; }

        public async Task<ActionResult> OnGetAsync(string name)
        {
            RequestedUser = await _context.Users.FirstOrDefaultAsync(u => u.NormalizedUserName == name.ToUpper());
            if (RequestedUser == null) return NotFound();
            var currentUser = User.FindFirstValue(ClaimTypes.NameIdentifier);

            IsCurrentUser = currentUser == RequestedUser.Id;
            
            var storiesQuery = RequestedUser.Id == currentUser
                ? _context.Stories.Where(s => s.Author.Id == currentUser)
                : _context.Stories.Where(s => s.Author == RequestedUser && s.IsPublished);
            
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
