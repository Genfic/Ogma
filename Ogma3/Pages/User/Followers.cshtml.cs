using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Ogma3.Data;
using Ogma3.Infrastructure.Extensions;
using Ogma3.Pages.Shared;
using Ogma3.Pages.Shared.Cards;

namespace Ogma3.Pages.User
{
    public class Followers : PageModel
    {
        private const int PerPage = 25;
        
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;

        public Followers(ApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public string Name { get; private set; }
        public Pagination Pagination { get; private set; }
        public List<UserCard> Users { get; private set; }

        public async Task<ActionResult> OnGetAsync(string name, [FromQuery] int page = 1)
        {
            Name = name;
            
            var uname = User.GetUsername()?.Normalize().ToUpperInvariant();
            if (uname is null) return NotFound();

            Users = await _context.FollowedUsers
                .Where(u => u.FollowingUser.NormalizedUserName == name.Normalize().ToUpperInvariant())
                .Select(u => u.FollowedUser)
                .Paginate(page, PerPage)
                .ProjectTo<UserCard>(_mapper.ConfigurationProvider)
                .AsNoTracking()
                .ToListAsync();

            var count = await _context.Users
                .Where(u => u.NormalizedUserName == name.Normalize().ToUpperInvariant())
                .Select(u => u.Followers)
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