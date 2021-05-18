using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Ogma3.Data;
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
        public GetData ClubThread { get; set; }
        
        public class GetData
        {
            public long Id { get; init; }
            public long ClubId { get; set; }
            public string Title { get; init; }
            public DateTime CreationDate { get; init; }
            public int Replies { get; init; }
        }

        public async Task<IActionResult> OnGetAsync(long id)
        {
            ClubThread = await _context.ClubThreads
                .Where(ct => ct.Id == id)
                .Where(ct => ct.AuthorId == User.GetNumericId())
                .Select(ct => new GetData
                {
                    Id = ct.Id,
                    ClubId = ct.ClubId,
                    Title = ct.Title,
                    CreationDate = ct.CreationDate,
                    Replies = ct.CommentsThread.CommentsCount
                })
                .AsNoTracking()
                .FirstOrDefaultAsync();

            if (ClubThread == null) return NotFound();
            
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(long id)
        {
            var uid = User.GetNumericId();
            if (uid is null) return Unauthorized();
            
            var thread = await _context.ClubThreads
                .Where(ct => ct.Id == id)
                .FirstOrDefaultAsync();

            if (thread is null) return NotFound();
            if (thread.AuthorId != uid) return Unauthorized();

            _context.ClubThreads.Remove(thread);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index", new { id = thread.ClubId });
        }
    }
}
