using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Ogma3.Data;
using Ogma3.Data.Roles;
using Ogma3.Infrastructure.Extensions;
using Ogma3.Pages.Shared.Bars;

namespace Ogma3.Pages.User
{
    public class IndexModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public IndexModel(ApplicationDbContext context)
        {
            _context = context;
        }
        
        public class UserProfileDto : ProfileBar
        {
            public string Bio { get; init; }
            public long CommentsThreadId { get; init; }
        }

        public UserProfileDto UserData { get; private set; }
        public async Task<IActionResult> OnGetAsync(string name)
        {
            var uid = User.GetNumericId();
            
            UserData = await _context.Users
                .Where(u => u.NormalizedUserName == name.Normalize().ToUpperInvariant())
                .Select(u => new UserProfileDto
                {
                    Id = u.Id,
                    UserName = u.UserName,
                    Avatar = u.Avatar,
                    Email = u.Email,
                    Title = u.Title,
                    Bio = u.Bio,
                    CommentsThreadId = u.CommentsThread.Id,
                    LastActive = u.LastActive,
                    RegistrationDate = u.RegistrationDate,
                    FollowersCount = u.Followers.Count,
                    BlogpostsCount = u.Blogposts.Count(b => b.IsPublished),
                    StoriesCount = u.Stories.Count(s => s.IsPublished),
                    IsBlockedBy = u.BlockedByUsers.Any(bu => bu.Id == uid),
                    IsFollowedBy = u.Followers.Any(fu => fu.Id == uid),
                    Roles = u.Roles.AsQueryable().Select(RoleMappings.ToRoleDto).ToList()
                })
                .FirstOrDefaultAsync();

            if (UserData is null) return NotFound();

            return Page();
        }

    }
}