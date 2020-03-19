using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Castle.Core.Internal;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Ogma3.Data;
using Ogma3.Data.Models;

namespace Ogma3.Areas.Profile.Pages
{
    public class Index : PageModel
    {
        private readonly ApplicationDbContext _context;
        
        public User CurrentUser { get; set; }
        
        public Index(ApplicationDbContext context)
        {
            _context = context;
        }
        
        public async Task<IActionResult> OnGetAsync(string? name)
        {
            var userName = name.IsNullOrEmpty()
                ? User.FindFirstValue(ClaimTypes.Name)
                : name;
            
            CurrentUser = await _context.Users
                .Where(u => u.NormalizedUserName == userName.ToUpper())
                .Include(u => u.CommentsThread)
                .FirstOrDefaultAsync();

            if (CurrentUser == null) return NotFound();
            return Page();
        }
    }
}