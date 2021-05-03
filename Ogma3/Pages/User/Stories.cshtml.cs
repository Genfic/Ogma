using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Ogma3.Data;
using Ogma3.Data.Stories;
using Ogma3.Data.Users;
using Ogma3.Infrastructure.Extensions;
using Ogma3.Pages.Shared;
using Ogma3.Pages.Shared.Bars;
using Ogma3.Pages.Shared.Cards;

namespace Ogma3.Pages.User
{
    public class StoriesModel : PageModel
    {
        private const int PerPage = 25;
        
        private readonly UserRepository _userRepo;
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;
        public StoriesModel(UserRepository userRepo, ApplicationDbContext context, IMapper mapper)
        {
            _userRepo = userRepo;
            _context = context;
            _mapper = mapper;
        }

        public IList<StoryCard> Stories { get; private set; }
        public ProfileBar ProfileBar { get; private set; }
        public Pagination Pagination { get; private set; }

        public async Task<ActionResult> OnGetAsync(string name, [FromQuery] int page = 1)
        {
            var uid = User.GetNumericId();
            
            ProfileBar = await _userRepo.GetProfileBar(name.ToUpper());
            if (ProfileBar is null) return NotFound();
            
            // Start building the query
            var query = _context.Stories
                .Where(b => b.AuthorId == ProfileBar.Id);

            if (ProfileBar.Id != uid)
            {   // If the profile page doesn't belong to the current user, apply additional filters
                query = query
                    .Where(b => b.IsPublished)
                    .Where(b => b.ContentBlockId == null)
                    .Blacklist(_context, uid);
            }

            // Resolve query
            Stories = await query
                .SortByEnum(EStorySortingOptions.DateDescending)
                .Paginate(page, PerPage)
                .ProjectTo<StoryCard>(_mapper.ConfigurationProvider)
                .AsNoTracking()
                .ToListAsync();

            // Prepare pagination
            Pagination = new Pagination
            {
                CurrentPage = page,
                ItemCount = await query.CountAsync(),
                PerPage = PerPage
            };
            
            return Page();
        }
    }
}
