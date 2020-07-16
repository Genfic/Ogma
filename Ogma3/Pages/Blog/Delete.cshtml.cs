using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Ogma3.Data;
using Ogma3.Data.Models;

namespace Ogma3.Pages.Blog
{
    [Authorize]
    public class DeleteModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private OgmaUserManager _userManager;

        public DeleteModel(ApplicationDbContext context, OgmaUserManager userManager)
        {
            _context = context;
            _userManager = userManager;
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
            var user = await _userManager.GetUserAsync(User);
            
            // Get post and make sure the user matches
            Blogpost = await _context.Blogposts
                .Where(b => b.Id == id && b.Author == user)
                .Include(b => b.CommentsThread)
                .FirstOrDefaultAsync();

            if (Blogpost == null) return RedirectToPage("./Index", new {name = user.UserName});

            _context.Blogposts.Remove(Blogpost);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index", new { name = Blogpost.Author.UserName });
        }
    }
}
