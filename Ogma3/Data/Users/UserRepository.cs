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

namespace Ogma3.Data.Users
{
    public class UserRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly long? _uid;
        private readonly IMapper _mapper;
        
        public UserRepository(ApplicationDbContext context, IUserService userService, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
            _uid = userService.GetUser()?.GetNumericId();
        }
        
        public async Task<ProfileBar> GetProfileBar(string name)
        {
            return await _context.Users
                .TagWith($"{nameof(UserRepository)}.{nameof(GetProfileBar)} -> {name}")
                .Where(u => u.NormalizedUserName == name.Normalize().ToUpper())
                .ProjectTo<ProfileBar>(_mapper.ConfigurationProvider)
                .AsNoTracking()
                .FirstOrDefaultAsync();
        }
        
        public async Task<ProfileBar> GetProfileBar(long id)
        {
            return await _context.Users
                .TagWith($"{nameof(UserRepository)}.{nameof(GetProfileBar)} -> {id}")
                .Where(u => u.Id == id)
                .ProjectTo<ProfileBar>(_mapper.ConfigurationProvider)
                .AsNoTracking()
                .FirstOrDefaultAsync();
        }

        public async Task<UserProfileDto> GetUserData(string name)
        {
            return await _context.Users
                .TagWith($"{nameof(UserRepository)}.{nameof(GetUserData)} -> {name}")
                .Where(u => u.NormalizedUserName == name.Normalize().ToUpper())
                .ProjectTo<UserProfileDto>(_mapper.ConfigurationProvider, new { currentUser = _uid })
                .AsNoTracking()
                .FirstOrDefaultAsync();
        }

        public async Task<UserProfileDto> GetUserData(long id)
        {
            return await _context.Users
                .TagWith($"{nameof(UserRepository)}.{nameof(GetUserData)} -> {id}")
                .Where(u => u.Id == id)
                .ProjectTo<UserProfileDto>(_mapper.ConfigurationProvider, new { currentUser = _uid })
                .AsNoTracking()
                .FirstOrDefaultAsync();
        }

        public async Task<ICollection<UserCard>> GetStaff()
        {
            return await _context.Users
                .TagWith($"{nameof(UserRepository)}.{nameof(GetStaff)}")
                .Where(u => u.Roles.Any(ur => ur.IsStaff))
                .ProjectTo<UserCard>(_mapper.ConfigurationProvider, new { currentUser = _uid })
                .OrderBy(uc => uc.Roles.OrderBy(r => r.Order).First().Order)
                .AsNoTracking()
                .ToListAsync();
        }
    }
}