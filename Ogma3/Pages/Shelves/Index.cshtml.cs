using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
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
        public bool DefaultUser { get; set; }

        public List<Icon> Icons { get; set; }
        
        public InputModel Input { get; set; }

        public class InputModel
        {
            [Required]
            [MinLength(CTConfig.Shelf.MinNameLength)]
            [MaxLength(CTConfig.Shelf.MaxNameLength)]
            public string Name { get; set; }
            
            [MaxLength(CTConfig.Shelf.MaxDescriptionLength)]
            public string Description { get; set; }
            
            [DisplayName("Public")]
            public bool IsPublic { get; set; }
            
            [DisplayName("Quick access")]
            public bool QuickAccess { get; set; }
            
            [MinLength(7)]
            [MaxLength(7)]
            public string Color { get; set; }

            public int Icon { get; set; }
        }

        public async Task<IActionResult> OnGetAsync(string? name)
        {
            Owner = name == null 
                ? await _userManager.GetUserAsync(User) 
                : await _context.Users.FirstAsync(u => u.NormalizedUserName == name.ToUpper());
            
            DefaultUser = name == null;

            Icons = await _context.Icons.ToListAsync();

            return Page();
        }
    }
}