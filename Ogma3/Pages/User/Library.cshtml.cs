using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Ogma3.Data;
using Ogma3.Data.Icons;
using Ogma3.Data.Users;
using Ogma3.Infrastructure.Extensions;
using Ogma3.Pages.Shared.Bars;

namespace Ogma3.Pages.User
{
    public class LibraryModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly UserRepository _userRepo;

        public LibraryModel(ApplicationDbContext context, UserRepository userRepo)
        {
            _context = context;
            _userRepo = userRepo;
        }

        public bool IsCurrentUser { get; private set; }
        public List<Icon> Icons { get; private set; }
        public InputModel Input { get; init; }
        public ProfileBar ProfileBar { get; private set; }

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
            ProfileBar = await _userRepo.GetProfileBar(name.ToUpper());
            if (ProfileBar is null) return NotFound();

            IsCurrentUser = ProfileBar.Id == User.GetNumericId();
            
            Icons = await _context.Icons
                .AsNoTracking()
                .ToListAsync();
            
            return Page();
        }

    }
}