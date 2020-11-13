using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using Ogma3.Data.DTOs;
using Ogma3.Data.Enums;
using Ogma3.Data.Models;
using Ogma3.Pages.Shared;
using Ogma3.Pages.Shared.Bars;
using Ogma3.Pages.Shared.Cards;
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
            if (!userId.HasValue) return false;
            
            return await _context.Clubs
                .TagWith($"{nameof(ClubRepository)} : {nameof(CheckRoles)} — {clubId}, {userId}")
                .Where(c => c.Id == clubId)
                .Where(c => c.ClubMembers
                    .Any(cm => cm.MemberId == userId && roles.Contains(cm.Role))
                )
                .AnyAsync();
        }

        public async Task<List<ClubCard>> SearchAndSortPaginatedClubCards(
            int page, 
            int perPage, 
            string query = null, 
            EClubSortingOptions sort = EClubSortingOptions.CreationDateDescending
        )
        {
            var q = _context.Clubs.AsQueryable();
            
            if (!string.IsNullOrEmpty(query))
                q = q.Where(c => EF.Functions.Like(c.Name.ToUpper(), $"%{query.Trim().ToUpper()}%"));

            return await q
                .SortByEnum(sort)
                .Paginate(page, perPage)
                .ProjectTo<ClubCard>(_mapper.ConfigurationProvider)
                .AsNoTracking()
                .ToListAsync();
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

        public async Task<int> CountClubs()
        {
            return await _context.Clubs.CountAsync();
        }

        public async Task<int> CountSearchedClubs(string query)
        {
            return await _context.Clubs
                .Where(c => EF.Functions.Like(c.Name.ToUpper(), $"%{query.Trim().ToUpper()}%"))
                .CountAsync();
        }
    }
}