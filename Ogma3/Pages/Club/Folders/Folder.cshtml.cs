using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Ogma3.Data;
using Ogma3.Data.Clubs;
using Ogma3.Infrastructure.Extensions;
using Ogma3.Pages.Shared;
using Ogma3.Pages.Shared.Bars;
using Ogma3.Pages.Shared.Cards;
using Ogma3.Pages.Shared.Minimals;

namespace Ogma3.Pages.Club.Folders
{
    public class FolderModel : PageModel
    {
        [NotNull]
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;
        private readonly ClubRepository _clubRepo;
        private readonly OgmaConfig _config;

        public FolderModel(ApplicationDbContext context, IMapper mapper, OgmaConfig config, ClubRepository clubRepo)
        {
            _context = context;
            _mapper = mapper;
            _config = config;
            _clubRepo = clubRepo;
        }

        public class FolderDetails
        {
            public long Id { get; init; }
            public string Name { get; init; }
            public string Slug { get; init; }
            public string Description { get; init; }
            public IEnumerable<FolderMinimal> ChildFolders { get; init; }
            public int StoriesCount { get; init; }
            public EClubMemberRoles AccessLevel { get; init; }
        }

        public ClubBar ClubBar { get; private set; }
        public FolderDetails Folder { get; private set; }
        public bool EditPermitted { get; private set; }
        public List<StoryCard> Stories { get; private set; }
        public Pagination Pagination { get; private set; }
        
        public async Task<IActionResult> OnGetAsync(long clubId, long id, [FromQuery] int page = 1)
        {
            var uid = User.GetNumericId();
            
            ClubBar = await _clubRepo.GetClubBar(clubId);
            if (ClubBar is null) return NotFound();

            Folder = await _context.Folders
                .Where(f => f.Id == id)
                .Select(f => new FolderDetails
                {
                    Id = f.Id,
                    Name = f.Name,
                    Slug = f.Slug,
                    Description = f.Description,
                    StoriesCount = f.StoriesCount,
                    AccessLevel = f.AccessLevel,
                    ChildFolders = f.ChildFolders.Select(cf => new FolderMinimal
                    {
                        Id = cf.Id,
                        ClubId = cf.ClubId,
                        Name = cf.Name,
                        Slug = cf.Slug,
                        StoriesCount = cf.StoriesCount
                    })
                })
                .AsNoTracking()
                .FirstOrDefaultAsync();
            if (Folder is null) return NotFound();

            EditPermitted = await _clubRepo.CheckRoles(ClubBar.Id, uid, new[]
            {
                EClubMemberRoles.Founder, 
                EClubMemberRoles.Admin
            });

            Stories = await _context.FolderStories
                .Where(s => s.FolderId == id)
                .Select(s => s.Story)
                .Where(b => b.IsPublished)
                .Where(b => b.ContentBlockId == null)
                .Blacklist(_context, uid)
                .OrderByDescending(s => s.ReleaseDate)
                .Paginate(page, _config.StoriesPerPage)
                .ProjectTo<StoryCard>(_mapper.ConfigurationProvider)
                .AsNoTracking()
                .ToListAsync();

            // Prepare pagination
            Pagination = new Pagination
            {
                PerPage = _config.StoriesPerPage,
                ItemCount = Folder.StoriesCount,
                CurrentPage = page
            };
            
            return Page();

        }
    }
}