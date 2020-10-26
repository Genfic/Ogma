using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using Ogma3.Data.DTOs;
using Ogma3.Pages.Shared;
using Ogma3.Services.UserService;
using Utils.Extensions;

namespace Ogma3.Data.Repositories
{
    public class UserRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;
        private readonly IUserService _userService;
        
        public UserRepository(ApplicationDbContext context, IMapper mapper, IUserService userService)
        {
            _context = context;
            _mapper = mapper;
            _userService = userService;
        }
        
        public async Task<ProfileBar> GetProfileBar(string name)
        {
            return await _context.Users
                .TagWith($"{nameof(UserRepository)}.{nameof(GetProfileBar)} -> {name}")
                .Where(u => u.NormalizedUserName == name.Normalize().ToUpper())
                .ProjectTo<ProfileBar>(_mapper.ConfigurationProvider, new { currentUser = _userService.GetUser()?.GetNumericId() })
                .AsNoTracking()
                .FirstOrDefaultAsync();
        }
        
        public async Task<ProfileBar> GetProfileBar(long id)
        {
            return await _context.Users
                .TagWith($"{nameof(UserRepository)}.{nameof(GetProfileBar)} -> {id}")
                .Where(u => u.Id == id)
                .ProjectTo<ProfileBar>(_mapper.ConfigurationProvider, new { currentUser = _userService.GetUser()?.GetNumericId() })
                .AsNoTracking()
                .FirstOrDefaultAsync();
        }

        public async Task<UserProfileDto> GetUserData(string name)
        {
            return await _context.Users
                .TagWith($"{nameof(UserRepository)}.{nameof(GetUserData)} -> {name}")
                .Where(u => u.NormalizedUserName == name.Normalize().ToUpper())
                .ProjectTo<UserProfileDto>(_mapper.ConfigurationProvider, new { currentUser = _userService.GetUser()?.GetNumericId() })
                .AsNoTracking()
                .FirstOrDefaultAsync();
        }

        public async Task<UserProfileDto> GetUserData(long id)
        {
            return await _context.Users
                .TagWith($"{nameof(UserRepository)}.{nameof(GetUserData)} -> {id}")
                .Where(u => u.Id == id)
                .ProjectTo<UserProfileDto>(_mapper.ConfigurationProvider, new { currentUser = _userService.GetUser()?.GetNumericId() })
                .AsNoTracking()
                .FirstOrDefaultAsync();
        }

        public async Task<ICollection<UserCard>> GetStaff()
        {
            return await _context.Users
                .TagWith($"{nameof(UserRepository)}.{nameof(GetStaff)}")
                .Where(u => u.Roles.Any(ur => ur.IsStaff))
                .ProjectTo<UserCard>(_mapper.ConfigurationProvider)
                .OrderBy(uc => uc.Roles.OrderBy(r => r.Order).First().Order)
                .AsNoTracking()
                .ToListAsync();
        }
    }
}