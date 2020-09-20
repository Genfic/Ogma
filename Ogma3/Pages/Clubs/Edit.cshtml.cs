using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Ogma3.Data;
using Ogma3.Data.Enums;
using Utils.Extensions;

namespace Ogma3.Pages.Clubs
{
    [Authorize]
    public class EditModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public EditModel(ApplicationDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Data.Models.Club Club { get; set; }

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

        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            _context.Attach(Club).State = EntityState.Modified;
            
            var founderId = await _context.ClubMembers
                .Where(cm => cm.ClubId == Club.Id && cm.Role == EClubMemberRoles.Founder)
                .Select(cm => cm.MemberId)
                .FirstOrDefaultAsync();

            if (User.IsUserSameAsLoggedIn(founderId)) return Unauthorized();

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ClubExists(Club.Id))
                {
                    return NotFound();
                }

                throw;
            }

            return RedirectToPage("/Club/Index", new { id = Club.Id });
        }

        private bool ClubExists(long id)
        {
            return _context.Clubs.Any(e => e.Id == id);
        }
    }
}
