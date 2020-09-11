using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Ogma3.Data;
using Ogma3.Data.DTOs;

namespace Ogma3.Pages
{
    public class ClubModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _config;

        public ClubModel(ApplicationDbContext context, IConfiguration config)
        {
            _context = context;
            _config = config;
        }

        public ClubBarDTO ClubBar { get; set; }
        
        public async Task<IActionResult> OnGetAsync(long id, string? slug)
        {
            ClubBar = await _context.Clubs
                .Where(c => c.Id == id)
                .Select(c => new ClubBarDTO
                {
                    Id = c.Id,
                    Name = c.Name,
                    Icon = c.Icon,
                    Description = c.Description,
                    Hook = c.Hook,
                    UserCount = c.ClubMembers.Count,
                    ThreadCount = c.Threads.Count,
                    StoryCount = 0
                })
                .AsNoTracking()
                .FirstOrDefaultAsync();

            if (ClubBar == null) return NotFound();

            return Page();
        }
    }
}