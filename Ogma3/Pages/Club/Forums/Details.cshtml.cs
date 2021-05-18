using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Ogma3.Data;
using Ogma3.Data.Clubs;
using Ogma3.Pages.Shared.Bars;
using Ogma3.Pages.Shared.Details;

namespace Ogma3.Pages.Club.Forums
{
    public class DetailsModel : PageModel
    {
        private readonly ClubRepository _clubRepo;
        private readonly ApplicationDbContext _context;

        public DetailsModel(ClubRepository clubRepo, ApplicationDbContext context)
        {
            _clubRepo = clubRepo;
            _context = context;
        }

        public ThreadDetails ClubThread { get; private set; }
        public ClubBar ClubBar { get; private set; }

        public async Task<IActionResult> OnGetAsync(long clubId, long threadId)
        {
            ClubBar = await _clubRepo.GetClubBar(clubId);
            if (ClubBar is null) return NotFound();
            
            ClubThread = await _context.ClubThreads
                .Where(ct => ct.Id == threadId)
                .Select(ct => new ThreadDetails
                {
                    Id = ct.Id,
                    ClubId = ct.ClubId,
                    Title = ct.Title,
                    CreationDate = ct.CreationDate,
                    AuthorName = ct.Author.UserName,
                    AuthorId = ct.Author.Id,
                    AuthorAvatar = ct.Author.Avatar,
                    AuthorRole = ct.Author.Roles
                        .Where(ur => ur.Order.HasValue)
                        .OrderBy(ur => ur.Order)
                        .First(),
                    Body = ct.Body,
                    CommentsThread = ct.CommentsThread
                })
                .AsNoTracking()
                .FirstOrDefaultAsync();

            if (ClubThread is null) return NotFound();
            
            return Page();
        }
    }
}
