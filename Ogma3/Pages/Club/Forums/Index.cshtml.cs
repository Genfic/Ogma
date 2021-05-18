using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LinqToDB;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Ogma3.Data;
using Ogma3.Data.Clubs;
using Ogma3.Pages.Shared;
using Ogma3.Pages.Shared.Bars;
using Ogma3.Pages.Shared.Cards;

namespace Ogma3.Pages.Club.Forums
{
    public class IndexModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly ClubRepository _clubRepo;
        private readonly OgmaConfig _config;

        public IndexModel(ApplicationDbContext context, ClubRepository clubRepo, OgmaConfig config)
        {
            _context = context;
            _clubRepo = clubRepo;
            _config = config;
        }

        public ClubBar ClubBar { get; private set; }
        public IList<ThreadCard> ThreadCards { get; private set; }
        public Pagination Pagination { get; private set; }

        public async Task<IActionResult> OnGetAsync(long id, [FromQuery] int page = 1)
        {
            ClubBar = await _clubRepo.GetClubBar(id);
            if (ClubBar is null) return NotFound();

            var query = _context.ClubThreads
                .Where(ct => ct.ClubId == id);

            ThreadCards = await query
                .OrderByDescending(ct => ct.CreationDate)
                .Paginate(page, _config.ClubThreadsPerPage)
                .Select(ct => new ThreadCard
                {
                    Id = ct.Id,
                    ClubId = ct.ClubId,
                    Title = ct.Title,
                    CreationDate = ct.CreationDate,
                    AuthorName = ct.Author.UserName,
                    AuthorAvatar = ct.Author.Avatar,
                    CommentsCount = ct.CommentsThread.Comments.Count
                })
                .ToListAsync();

            Pagination = new Pagination
            {
                ItemCount = await query.CountAsync(),
                CurrentPage = page,
                PerPage = _config.ClubThreadsPerPage
            };

            return Page();
        }
    }
}