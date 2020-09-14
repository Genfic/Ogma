using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Ogma3.Data;
using Ogma3.Data.DTOs;
using Ogma3.Data.Models;
using Ogma3.Data.Repositories;
using Ogma3.Pages.Shared;

namespace Ogma3.Pages.User
{
    public class IndexModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private UserRepository _userRepo;

        public string Bio { get; set; }
        public CommentsThread CommentsThread { get; set; }
        public ProfileBar ProfileBar { get; set; }

        public IndexModel(ApplicationDbContext context, UserRepository userRepo)
        {
            _context = context;
            _userRepo = userRepo;
        }

        public async Task<IActionResult> OnGetAsync(string name)
        {
            var userData = await _context.Users
                .Where(u => u.NormalizedUserName == name.ToUpper())
                .Select(u => new {u.Bio, u.CommentsThread })
                .AsNoTracking()
                .FirstOrDefaultAsync();

            if (userData == null) return NotFound();
            
            Bio = userData.Bio;
            CommentsThread = userData.CommentsThread;
            
            ProfileBar = await _userRepo.GetProfileBar(name.ToUpper());

            if (ProfileBar == null) return NotFound();
            
            return Page();
        }

    }
}