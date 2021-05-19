using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using Ogma3.Infrastructure.Extensions;
using Ogma3.Pages.Shared.Bars;
using Ogma3.Services.UserService;

namespace Ogma3.Data.Clubs
{
    public class ClubRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;
        private readonly long? _uid;

        public ClubRepository(ApplicationDbContext context, IMapper mapper, IUserService userService)
        {
            _context = context;
            _mapper = mapper;
            _uid = userService.GetUser()?.GetNumericId();
        }

        public async Task<ClubBar> GetClubBar(long clubId)
        {
            return await _context.Clubs
                .Where(c => c.Id == clubId)
                .ProjectTo<ClubBar>(_mapper.ConfigurationProvider, new { currentUser = _uid })
                .AsNoTracking()
                .FirstOrDefaultAsync();
        }

        public async Task<List<UserClubMinimalDto>> GetUserClubsMinimal(long userId)
        {
            return await _context.Clubs
                .Where(c => c.ClubMembers.Any(cm => cm.MemberId == userId))
                .OrderBy(c => c.Name)
                .Select(c => new UserClubMinimalDto
                {
                    Id = c.Id,
                    Name = c.Name,
                    Icon = c.Icon
                })
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<bool> CheckRoles(long clubId, long? userId, IEnumerable<EClubMemberRoles> roles)
        {
            if (userId is null) return false;
            
            return await _context.Clubs
                .TagWith($"{nameof(ClubRepository)} : {nameof(CheckRoles)} — {clubId}, {userId}")
                .Where(c => c.Id == clubId)
                .Where(c => c.ClubMembers
                    .Any(cm => cm.MemberId == userId && roles.Contains(cm.Role))
                )
                .AnyAsync();
        }

        public async Task<List<ClubMinimalDto>> GetClubsWithStory(long storyId)
        {
            return await _context.Clubs
                .Where(c => c.Folders
                    .Any(f => f.Stories
                        .Any(s => s.Id == storyId)
                    )
                )
                .Select(c => new ClubMinimalDto
                {
                    Id = c.Id,
                    Name = c.Name,
                    Icon = c.Icon
                })
                .ToListAsync();
        }
    }
}