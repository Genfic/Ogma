using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using Ogma3.Pages.Shared;
using Ogma3.Services.UserService;
using Utils.Extensions;

namespace Ogma3.Data.Repositories
{
    public class ClubRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;
        private readonly IUserService _userService;

        public ClubRepository(ApplicationDbContext context, IMapper mapper, IUserService userService)
        {
            _context = context;
            _mapper = mapper;
            _userService = userService;
        }

        public async Task<ClubBar> GetClubBar(long clubId)
        {
            return await _context.Clubs
                .Where(c => c.Id == clubId)
                .ProjectTo<ClubBar>(_mapper.ConfigurationProvider, new { currentUser = _userService.GetUser()?.GetNumericId() })
                .AsNoTracking()
                .FirstOrDefaultAsync();
        }

        public async Task<List<UserCard>> GetMembers(long clubId, int page, int perPage)
        {
            return await _context.ClubMembers
                .Where(cm => cm.ClubId == clubId)
                .Select(cm => cm.Member)
                .ProjectTo<UserCard>(_mapper.ConfigurationProvider)
                .Paginate(page, perPage)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<List<ClubCard>> GetPaginatedClubCards(int page, int perPage)
        {
            return await _context.Clubs
                .Paginate(page, perPage)
                .ProjectTo<ClubCard>(_mapper.ConfigurationProvider)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<int> CountClubs()
        {
            return await _context.Clubs.CountAsync();
        }
    }
}