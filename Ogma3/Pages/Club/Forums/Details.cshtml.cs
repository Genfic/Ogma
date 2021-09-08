using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Ogma3.Data;
using Ogma3.Data.Clubs;
using Ogma3.Data.Roles;
using Ogma3.Pages.Shared;
using Ogma3.Pages.Shared.Bars;

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
        
        public class ThreadDetails
        {
            public long Id { get; init; }
            public long ClubId { get; init; }
            public string Title { get; init; }
            public string Body { get; init; }
            public bool IsPinned { get; init; }
            public DateTime CreationDate { get; init; }
            public string AuthorName { get; init; }
            public long AuthorId { get; init; }
            public string AuthorAvatar { get; init; }
            public OgmaRole AuthorRole { get; init; }
            public CommentsThreadDto CommentsThread { get; init; }
        }

        public ThreadDetails ClubThread { get; private set; }
        public ClubBar ClubBar { get; private set; }

        public async Task<IActionResult> OnGetAsync(long threadId)
        {
            ClubThread = await _context.ClubThreads
                .Where(ct => ct.Id == threadId)
                .Select(ct => new ThreadDetails
                {
                    Id = ct.Id,
                    ClubId = ct.ClubId,
                    Title = ct.Title,
                    IsPinned = ct.IsPinned,
                    CreationDate = ct.CreationDate,
                    AuthorName = ct.Author.UserName,
                    AuthorId = ct.Author.Id,
                    AuthorAvatar = ct.Author.Avatar,
                    AuthorRole = ct.Author.Roles
                        .Where(ur => ur.Order.HasValue)
                        .OrderBy(ur => ur.Order)
                        .First(),
                    Body = ct.Body,
                    CommentsThread = new CommentsThreadDto
                    {
                        Id = ct.CommentsThread.Id,
                        LockDate = ct.CommentsThread.LockDate
                    }
                })
                .AsNoTracking()
                .FirstOrDefaultAsync();

            if (ClubThread is null) return NotFound();

            ClubThread.CommentsThread.Type = nameof(Data.ClubThreads.ClubThread);
            
            ClubBar = await _clubRepo.GetClubBar(ClubThread.ClubId);
            if (ClubBar is null) return NotFound();
            
            return Page();
        }
    }
}
