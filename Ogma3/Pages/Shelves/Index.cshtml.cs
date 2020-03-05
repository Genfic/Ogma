using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
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
        
        public InputModel Input { get; set; }

        public class InputModel
        {
            public string Name { get; set; }
            public string Description { get; set; }
            
            [DisplayName("Public")]
            public bool IsPublic { get; set; }
            
            [DisplayName("Quick access")]
            public bool QuickAccess { get; set; }
            public string Color { get; set; }
        }

        public async Task<IActionResult> OnGetAsync(string? name)
        {
            Owner = name == null 
                ? await _userManager.GetUserAsync(User) 
                : await _context.Users.FirstAsync(u => u.NormalizedUserName == name.ToUpper());
            
            UserShelves = await _context.Shelves.Where(s => s.Owner == Owner).ToListAsync();

            return Page();
        }
    }
}