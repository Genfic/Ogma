using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Ogma3.Data.DTOs;

namespace Ogma3.Data.Repositories
{
    public class ProfileBarRepository
    {
        private readonly ApplicationDbContext _context;
        
        public ProfileBarRepository(ApplicationDbContext context)
        {
            _context = context;
        }
        
        public async Task<ProfileBarDTO> GetAsync(string normalizedName)
        {
            return await _context.Users
                .Where(u => u.NormalizedUserName == normalizedName.Normalize())
                .Select(u => new ProfileBarDTO
                {
                    Id = u.Id,
                    UserName = u.UserName,
                    Title = u.Title,
                    Avatar = u.Avatar,
                    Email = u.Email,
                    RegistrationDate = u.RegistrationDate,
                    LastActive = u.LastActive,
                    StoriesCount = _context.Stories.Count(s => s.Author.Id == u.Id),
                    BlogpostsCount = _context.Blogposts.Count(b => b.Author.Id == u.Id)
                })
                .AsNoTracking()
                .FirstOrDefaultAsync();
        }
    }
}