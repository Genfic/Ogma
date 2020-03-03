using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Ogma3.Data;
using Ogma3.Data.Models;

namespace Ogma3.Pages.Shelves
{
    public class Index : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<User> _userManager;

        public Index(ApplicationDbContext context, UserManager<User> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public User Owner { get; set; }
        public List<Shelf> UserShelves { get; set; }

        public async void OnGetAsync(string? name)
        {
            Owner = name == null 
                ? await _userManager.GetUserAsync(User) 
                : await _context.Users.FirstAsync(u => u.NormalizedUserName == name.ToUpper());
            UserShelves = await _context.Shelves.Where(s => s.Owner == Owner).ToListAsync();
        }
    }
}