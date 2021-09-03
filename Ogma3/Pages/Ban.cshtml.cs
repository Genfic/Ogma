using System;
using System.Linq;
using System.Threading.Tasks;
using LinqToDB;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Ogma3.Data;
using Ogma3.Infrastructure.Extensions;

namespace Ogma3.Pages
{
    public class Ban : PageModel
    {
        private readonly ApplicationDbContext _context;
        public Ban(ApplicationDbContext context) => _context = context;

        public DateTime? BannedUntil { get; set; }
        
        public async Task<IActionResult> OnGetAsync()
        {
            var uid = User.GetNumericId();
            if (uid is null) return Unauthorized();

            BannedUntil = await _context.Users
                .Where(u => u.Id == uid)
                .Select(u => u.BannedUntil)
                .FirstOrDefaultAsync();

            return Page();
        }
    }
}