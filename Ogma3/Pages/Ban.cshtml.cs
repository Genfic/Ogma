using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LinqToDB;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Ogma3.Data;
using Ogma3.Data.Infractions;
using Ogma3.Infrastructure.Extensions;

namespace Ogma3.Pages
{
    public class Ban : PageModel
    {
        private readonly ApplicationDbContext _context;
        public Ban(ApplicationDbContext context) => _context = context;

        public DateTime BannedUntil { get; private set; }
        public List<InfractionDto> Infractions { get; private set; }
        
        public async Task<IActionResult> OnGetAsync()
        {
            var uid = User.GetNumericId();
            if (uid is null) return Unauthorized();

            Infractions = await _context.Infractions
                .Where(i => i.UserId == uid)
                .Where(i => i.Type != InfractionType.Note)
                .OrderByDescending(i => i.Type)
                .ThenByDescending(i => i.ActiveUntil)
                .Select(i => new InfractionDto
                {
                    IssueDate = i.IssueDate,
                    ActiveUntil = i.ActiveUntil,
                    RemovedAt = i.RemovedAt,
                    Reason = i.Reason,
                    Type = i.Type
                })
                .ToListAsync();
            
            BannedUntil = Infractions
                .Where(i => i.RemovedAt == null)
                .Where(i => i.ActiveUntil > DateTime.Now)
                .OrderByDescending(i => i.ActiveUntil)
                .Select(i => i.ActiveUntil)
                .FirstOrDefault();

            if (BannedUntil == default)
            {
                return RedirectToPage("/Index");
            }

            return Page();
        }

        public sealed record InfractionDto
        {
            public DateTime IssueDate { get; init; }
            public DateTime ActiveUntil { get; init; }
            public DateTime? RemovedAt { get; init; }
            public string Reason { get; init; }
            public InfractionType Type { get; init; }
        }
    }
}