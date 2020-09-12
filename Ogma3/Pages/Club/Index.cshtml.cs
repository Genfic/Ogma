using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Ogma3.Data;
using Ogma3.Data.DTOs;
using Ogma3.Data.Models;

namespace Ogma3.Pages.Club
{
    public class IndexModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public IndexModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public ClubBarDTO ClubBar { get; set; }
        public IList<ClubThread> Threads { get; set; }
        
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
                    StoryCount = c.ClubStories.Count
                })
                .AsNoTracking()
                .FirstOrDefaultAsync();
            
            if (ClubBar == null) return NotFound();

            Threads = await _context.ClubThreads
                .Where(ct => ct.ClubId == id)
                .OrderByDescending(ct => ct.CreationDate)
                .Take(5)
                .Include(ct => ct.CommentsThread)
                .AsNoTracking()
                .ToListAsync();

            return Page();
        }
    }
}