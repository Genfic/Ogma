using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Ogma3.Data;
using Ogma3.Data.Models;
using Utils.Extensions;

namespace Ogma3.Areas.Identity.Pages.Account.Manage
{
    public class Blacklists : PageModel
    {
        private readonly ApplicationDbContext _context;

        public Blacklists(ApplicationDbContext context)
        {
            _context = context;
        }
        public IEnumerable<Rating> Ratings { get; set; }
        
        public async Task<IActionResult> OnGetAsync()
        {
            Ratings = await _context.Ratings.ToListAsync();
            BlacklistedRatings = await _context.BlacklistedRatings
                .Where(br => br.UserId == User.GetNumericId())
                .Select(br => br.RatingId)
                .ToListAsync();

            return Page();
        }

        [BindProperty]
        public IEnumerable<long> BlacklistedRatings { get; set; }
        
        public async Task<IActionResult> OnPostAsync()
        {
            var uid = User.GetNumericId();
            if (uid == null) return Unauthorized();

            var user = await _context.Users
                .Where(u => u.Id == uid)
                .Include(u => u.BlacklistedRatings)
                .FirstOrDefaultAsync();
            if (user == null) return Unauthorized();
            
            // Clear blacklisted ratings
            _context.BlacklistedRatings.RemoveRange(user.BlacklistedRatings);

            await _context.SaveChangesAsync();
            
            // Add blacklisted ratings
            user.BlacklistedRatings = BlacklistedRatings
                .Select(rating => new BlacklistedRating
                {
                    RatingId = rating, 
                    UserId = (long) uid
                })
                .ToList();

            await _context.SaveChangesAsync();
            
            return RedirectToPage();
        }
    }
}