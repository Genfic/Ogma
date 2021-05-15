using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using Ogma3.Infrastructure.Extensions;
using Ogma3.Pages.Shared.Bars;
using Ogma3.Pages.Shared.Cards;
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
            if (userId is null) return false;
            
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

            return await (sort switch
                {
                    EClubSortingOptions.NameAscending => q.OrderBy(c => c.Name),
                    EClubSortingOptions.NameDescending => q.OrderByDescending(c => c.Name),
                    EClubSortingOptions.MembersAscending => q.OrderBy(c => c.ClubMembers.Count),
                    EClubSortingOptions.MembersDescending => q.OrderByDescending(c => c.ClubMembers.Count),
                    EClubSortingOptions.StoriesAscending => q.OrderBy(c => c.Folders.Sum(f => f.StoriesCount)),
                    EClubSortingOptions.StoriesDescending => q.OrderByDescending(c => c.Folders.Sum(f => f.StoriesCount)),
                    EClubSortingOptions.ThreadsAscending => q.OrderBy(c => c.Threads.Count),
                    EClubSortingOptions.ThreadsDescending => q.OrderByDescending(c => c.Threads.Count),
                    EClubSortingOptions.CreationDateAscending => q.OrderBy(c => c.CreationDate),
                    EClubSortingOptions.CreationDateDescending => q.OrderByDescending(c => c.CreationDate),
                    _ => q.OrderByDescending(c => c.CreationDate)
                })
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