using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Ogma3.Data;
using Ogma3.Data.Icons;
using Ogma3.Infrastructure.Extensions;

namespace Ogma3.Pages.User
{
    public class LibraryModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public LibraryModel(ApplicationDbContext context)
        {
            _context = context;
        }

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

        public string Name { get; set; }
        public bool IsOwner { get; set; }
        public List<Icon> Icons { get; private set; }
        public InputModel Input { get; init; }

        public async Task<IActionResult> OnGetAsync(string name)
        {            
            Name = name;
            
            var uname = User.GetUsername()?.Normalize().ToUpperInvariant();
            if (uname is null) return NotFound();

            IsOwner = string.Equals(name, uname, StringComparison.InvariantCultureIgnoreCase);
            
            Icons = await _context.Icons
                .AsNoTracking()
                .ToListAsync();
            
            return Page();
        }

    }
}