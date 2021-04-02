using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Ogma3.Data;
using Ogma3.Data.Users;
using Ogma3.Pages.Shared;
using Ogma3.Pages.Shared.Bars;
using Ogma3.Pages.Shared.Cards;

namespace Ogma3.Pages.User
{
    public class Follows : PageModel
    {
        private const int PerPage = 25;
        
        private readonly UserRepository _userRepo;
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;

        public Follows(UserRepository userRepo, ApplicationDbContext context, IMapper mapper)
        {
            _userRepo = userRepo;
            _context = context;
            _mapper = mapper;
        }
        
        public ProfileBar ProfileBar { get; set; }
        public Pagination Pagination { get; set; }
        
        public List<UserCard> Users { get; set; }

        public async Task<ActionResult> OnGetAsync(string name, [FromQuery] int page = 1)
        {
            ProfileBar = await _userRepo.GetProfileBar(name.ToUpper());
            if (ProfileBar == null) return NotFound();

            Users = await _context.FollowedUsers
                .Where(u => u.FollowedUser.NormalizedUserName == name.ToUpper())
                .Select(u => u.FollowingUser)
                .ProjectTo<UserCard>(_mapper.ConfigurationProvider)
                .Paginate(page, PerPage)
                .AsNoTracking()
                .ToListAsync();

            var count = await _context.Users
                .Where(u => u.NormalizedUserName == name.ToUpper())
                .Select(u => u.Following)
                .CountAsync();
            
            // Prepare pagination
            Pagination = new Pagination
            {
                PerPage = PerPage,
                ItemCount = count,
                CurrentPage = page
            };
            
            return Page();
        }
    }
}