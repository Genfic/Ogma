using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Ogma3.Data;
using Ogma3.Data.Models;

namespace Ogma3.Pages.User
{
    public class LibraryModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public LibraryModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public OgmaUser Owner { get; set; }
        public bool IsCurrentUser { get; set; }
        public List<Icon> Icons { get; set; }
        public InputModel Input { get; set; }

        public class InputModel
        {
            [Required]
            [MinLength(CTConfig.CShelf.MinNameLength)]
            [MaxLength(CTConfig.CShelf.MaxNameLength)]
            public string Name { get; set; }
            
            [MaxLength(CTConfig.CShelf.MaxDescriptionLength)]
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

        public async Task<IActionResult> OnGetAsync(string name)
        {
            Owner = await _context.Users.FirstAsync(u => u.NormalizedUserName == name.ToUpper());
            IsCurrentUser = Owner.Id.ToString() == User.FindFirstValue(ClaimTypes.NameIdentifier);
            Icons = await _context.Icons
                .AsNoTracking()
                .ToListAsync();

            return Page();
        }
    }
}