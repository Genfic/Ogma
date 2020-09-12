using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Ogma3.Data.DTOs;
using Ogma3.Pages.Shared;

namespace Ogma3.Data.Repositories
{
    public class ClubRepository
    {
        private readonly ApplicationDbContext _context;

        public ClubRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<ClubBar> GetClubBar(long clubId, long userId)
        {
            return await _context.Clubs
                .Where(c => c.Id == clubId)
                .Select(c => new ClubBar
                {
                    Id = c.Id,
                    Name = c.Name,
                    Icon = c.Icon,
                    Description = c.Description,
                    Hook = c.Hook,
                    UserCount = c.ClubMembers.Count,
                    ThreadCount = c.Threads.Count,
                    StoryCount = c.ClubStories.Count,
                    CreationDate = c.CreationDate,
                    IsMember = c.ClubMembers.Any(cm => cm.MemberId == userId)
                })
                .AsNoTracking()
                .FirstOrDefaultAsync();
        }
    }
}