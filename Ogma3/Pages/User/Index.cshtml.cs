using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Ogma3.Data;
using Ogma3.Data.DTOs;
using Ogma3.Data.Models;
using Ogma3.Data.Repositories;

namespace Ogma3.Pages.User
{
    public class IndexModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private ProfileBarRepository _profileBarRepo;

        public string Bio { get; set; }
        public CommentsThread CommentsThread { get; set; }
        public ProfileBarDTO ProfileBar { get; set; }

        public IndexModel(ApplicationDbContext context, ProfileBarRepository profileBarRepo)
        {
            _context = context;
            _profileBarRepo = profileBarRepo;
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
            
            ProfileBar = await _profileBarRepo.GetAsync(name.ToUpper());

            if (ProfileBar == null) return NotFound();
            
            return Page();
        }

    }
}