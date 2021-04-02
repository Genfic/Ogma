using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Ogma3.Data;
using Ogma3.Data.Clubs;
using Ogma3.Infrastructure.Extensions;

namespace Ogma3.Pages.Club.Forums
{
    public class DeleteModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public DeleteModel(ApplicationDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public ClubThread ClubThread { get; set; }

        public async Task<IActionResult> OnGetAsync(long id)
        {
            ClubThread = await _context.ClubThreads
                .Where(ct => ct.Id == id)
                .Where(ct => ct.AuthorId == User.GetNumericId())
                .AsNoTracking()
                .FirstOrDefaultAsync();

            if (ClubThread == null) return NotFound();
            
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(long id)
        {
            ClubThread = await _context.ClubThreads
                .Where(ct => ct.Id == id)
                .Where(ct => ct.AuthorId == User.GetNumericId())
                .FirstOrDefaultAsync();

            if (ClubThread == null) return NotFound();

            _context.ClubThreads.Remove(ClubThread);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index", new { id = ClubThread.ClubId });
        }
    }
}
