using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Ogma3.Data;
using Ogma3.Infrastructure.Extensions;

namespace Ogma3.Pages.User
{
    public class IndexModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public IndexModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public class ProfileModel
        {
            public string Bio { get; init; }
            public long CommentsThreadId { get; init; }
        }

        public string Name { get; private set; }
        public ProfileModel UserData { get; private set; }
        public async Task<IActionResult> OnGetAsync(string name)
        {
            Name = name;
            
            var uname = User.GetUsername()?.Normalize().ToUpperInvariant();
            var uid = User.GetNumericId();
            if (uname is null || uid is null) return NotFound();
            
            UserData = await _context.Users
                .Where(u => u.NormalizedUserName == name.Normalize().ToUpperInvariant())
                .Select(u => new ProfileModel
                {
                    Bio = u.Bio,
                    CommentsThreadId = u.CommentsThread.Id
                })
                .FirstOrDefaultAsync();

            if (UserData is null) return NotFound();

            return Page();
        }

    }
}