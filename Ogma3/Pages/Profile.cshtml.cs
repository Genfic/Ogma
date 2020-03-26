using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Ogma3.Data;
using Ogma3.Data.Models;

namespace Ogma3.Pages
{
    public class Profile : PageModel
    {
        private readonly ApplicationDbContext _context;
        public User CurrentUser { get; set; }

        public Profile(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> OnGetAsync(string name)
        {
            CurrentUser = await _context.Users
                .Where(u => u.NormalizedUserName == name.ToUpper())
                .Include(u => u.CommentsThread)
                .FirstOrDefaultAsync();

            if (CurrentUser == null) return NotFound();
            return Page();
        }
    }
}