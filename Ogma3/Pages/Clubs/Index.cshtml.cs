using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Ogma3.Data;
using Ogma3.Data.DTOs;
using Ogma3.Pages.Shared;

namespace Ogma3.Pages.Clubs
{
    public class IndexModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public IndexModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public IList<ClubCard> Clubs { get;set; }

        public const int PerPage = 2;
        public Pagination Pagination { get; set; }
        public async Task OnGetAsync()
        {
            Clubs = await _context.Clubs
                .Select(c => new ClubCard
                {
                    Id = c.Id,
                    Name = c.Name,
                    Hook = c.Hook,
                    Icon = c.Icon,
                    StoryCount = 0,
                    ThreadCount = c.Threads.Count,
                    UserCount = c.ClubMembers.Count
                })
                .AsNoTracking()
                .ToListAsync();
        }
    }
}
