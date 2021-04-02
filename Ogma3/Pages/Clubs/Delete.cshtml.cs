using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Ogma3.Data;
using Ogma3.Data.Clubs;
using Ogma3.Infrastructure.Extensions;

namespace Ogma3.Pages.Clubs
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
        public Data.Clubs.Club Club { get; set; }

        public async Task<IActionResult> OnGetAsync(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var club = await _context.Clubs
                .Where(c => c.Id == id)
                .Select(c => new
                {
                    Club = c,
                    FounderId = c.ClubMembers.First(cm => cm.Role == EClubMemberRoles.Founder).MemberId
                })
                .AsNoTracking()
                .FirstOrDefaultAsync();

            if (club == null) return NotFound();
            
            Club = club.Club;

            if (Club == null) return NotFound();

            if (!User.IsUserSameAsLoggedIn(club.FounderId)) return Unauthorized();
            
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Club = await _context.Clubs
                .Where(c => c.Id == id)
                .Include(c => c.ClubMembers)
                .FirstOrDefaultAsync();
            
            if (Club == null) return NotFound();

            var founderId = await _context.ClubMembers
                .Where(cm => cm.ClubId == Club.Id && cm.Role == EClubMemberRoles.Founder)
                .Select(cm => cm.MemberId)
                .FirstOrDefaultAsync();

            if (User.IsUserSameAsLoggedIn(founderId)) return Unauthorized();
            
            _context.Clubs.Remove(Club);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}
