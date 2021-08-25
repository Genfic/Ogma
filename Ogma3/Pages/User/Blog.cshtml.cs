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
using Ogma3.Infrastructure.Extensions;
using Ogma3.Pages.Shared;
using Ogma3.Pages.Shared.Bars;
using Ogma3.Pages.Shared.Cards;

namespace Ogma3.Pages.User
{
    public class BlogModel : PageModel
    {
        private const int PerPage = 25;
        
        private readonly UserRepository _userRepo;
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;
        public BlogModel(UserRepository userRepo, ApplicationDbContext context, IMapper mapper)
        {
            _userRepo = userRepo;
            _context = context;
            _mapper = mapper;
        }
        
        public ICollection<BlogpostCard> Posts { get; private set; }
        public ProfileBar ProfileBar { get; private set; }
        public Pagination Pagination { get; private set; }
        
        public async Task<ActionResult> OnGetAsync(string name, [FromQuery] int page = 1)
        {
            var uid = User.GetNumericId();
            
            ProfileBar = await _userRepo.GetProfileBar(name);
            if (ProfileBar is null) return NotFound();

            // Start building the query
            var query = _context.Blogposts
                .Where(b => b.AuthorId == ProfileBar.Id);
            
            if (uid != ProfileBar.Id)
            {   // If the profile page doesn't belong to the current user, apply additional filters
                query = query
                    .Where(b => b.PublicationDate != null)
                    .Where(b => b.ContentBlockId == null);
            }

            // Resolve query
            Posts = await query
                .OrderByDescending(b => b.CreationDate)
                .Paginate(page, PerPage)
                .ProjectTo<BlogpostCard>(_mapper.ConfigurationProvider)
                .AsNoTracking()
                .ToListAsync();
            
            // Prepare pagination
            Pagination = new Pagination
            {
                PerPage = PerPage,
                ItemCount = await query.CountAsync(),
                CurrentPage = page
            };
            
            return Page();
        }

    }
}
