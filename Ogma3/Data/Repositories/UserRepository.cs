using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Ogma3.Data.DTOs;
using Ogma3.Pages.Shared;

namespace Ogma3.Data.Repositories
{
    public class UserRepository
    {
        private readonly ApplicationDbContext _context;
        
        public UserRepository(ApplicationDbContext context)
        {
            _context = context;
        }
        
        public async Task<ProfileBar> GetProfileBar(string name)
        {
            return await _context.Users
                .Where(u => u.NormalizedUserName == name.Normalize())
                .Select(u => new ProfileBar
                {
                    Id = u.Id,
                    UserName = u.UserName,
                    Title = u.Title,
                    Avatar = u.Avatar,
                    Email = u.Email,
                    RegistrationDate = u.RegistrationDate,
                    LastActive = u.LastActive,
                    
                    Roles = u.UserRoles.Select(ur => new RoleDTO
                    {
                        Id = ur.RoleId,
                        Name = ur.Role.Name,
                        IsStaff = ur.Role.IsStaff,
                        Color = ur.Role.Color
                    }),
                    
                    StoriesCount = _context.Stories
                        .Where(s => s.IsPublished)
                        .Count(s => s.Author.Id == u.Id),
                    BlogpostsCount = _context.Blogposts
                        .Where(b => b.IsPublished)
                        .Count(b => b.Author.Id == u.Id)
                })
                .AsNoTracking()
                .TagWith("Fetch profile bar data")
                .FirstOrDefaultAsync();
        }
        
        public async Task<ProfileBar> GetProfileBar(long id)
        {
            return await _context.Users
                .Where(u => u.Id == id)
                .Select(u => new ProfileBar
                {
                    Id = u.Id,
                    UserName = u.UserName,
                    Title = u.Title,
                    Avatar = u.Avatar,
                    Email = u.Email,
                    RegistrationDate = u.RegistrationDate,
                    LastActive = u.LastActive,
                    
                    Roles = u.UserRoles.Select(ur => new RoleDTO
                    {
                        Id = ur.RoleId,
                        Name = ur.Role.Name,
                        IsStaff = ur.Role.IsStaff,
                        Color = ur.Role.Color
                    }),
                    
                    StoriesCount = _context.Stories
                        .Where(s => s.IsPublished)
                        .Count(s => s.Author.Id == u.Id),
                    BlogpostsCount = _context.Blogposts
                        .Where(b => b.IsPublished)
                        .Count(b => b.Author.Id == u.Id)
                })
                .AsNoTracking()
                .TagWith("Fetch profile bar data")
                .FirstOrDefaultAsync();
        }
    }
}