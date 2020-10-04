using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Ogma3.Data;
using Ogma3.Data.Models;
using Utils.Extensions;

namespace Ogma3.Pages.Blog
{
    [Authorize]
    public class DeleteModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public DeleteModel(ApplicationDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Blogpost Blogpost { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Blogpost = await _context.Blogposts
                .Where(m => m.Id == id)
                .Include(b => b.CommentsThread)
                .AsNoTracking()
                .FirstOrDefaultAsync();

            if (Blogpost == null)
            {
                return NotFound();
            }
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            
            // Get logged in user
            var uid = User.GetNumericId();
            var uname = User.GetUsername();

            if (uid == null || uname == null) return Unauthorized();
            
            // Get post and make sure the user matches
            Blogpost = await _context.Blogposts
                .Where(b => b.Id == id && b.AuthorId == uid)
                .Include(b => b.CommentsThread)
                .FirstOrDefaultAsync();

            if (Blogpost == null) return RedirectToPage("/User/Blog", new { name = uname });

            _context.Blogposts.Remove(Blogpost);
            await _context.SaveChangesAsync();

            return RedirectToPage("/User/Blog", new { name = uname });
        }
    }
}
