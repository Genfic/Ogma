using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Ogma3.Data;
using Ogma3.Data.Clubs;
using Ogma3.Data.Folders;
using Ogma3.Pages.Shared.Bars;
using Ogma3.Pages.Shared.Cards;

namespace Ogma3.Pages.Club.Folders
{
    public class IndexModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly ClubRepository _clubRepo;

        public IndexModel(ClubRepository clubRepo, ApplicationDbContext context)
        {
            _clubRepo = clubRepo;
            _context = context;
        }
        
        public ClubBar ClubBar { get; private set; }
        public ICollection<FolderCard> Folders { get; private set; }

        public async Task<IActionResult> OnGetAsync(long id)
        {
            ClubBar = await _clubRepo.GetClubBar(id);
            if (ClubBar == null) return NotFound();

            Folders = await _context.Folders
                .Where(f => f.ClubId == id)
                .Where(f => f.ParentFolderId == null)
                .Select(f => new FolderCard
                {
                    Id = f.Id,
                    Name = f.Name,
                    Slug = f.Slug,
                    Description = f.Description,
                    ClubId = f.ClubId,
                    StoriesCount = f.StoriesCount,
                    ChildFolders = f.ChildFolders.Select(cf => new FolderMinimalDto
                    {
                        Id = cf.Id,
                        Name = cf.Name,
                        Slug = cf.Slug
                    })
                })
                .AsNoTracking()
                .ToListAsync();
            
            return Page();

        }
    }
}