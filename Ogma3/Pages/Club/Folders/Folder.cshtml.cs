using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Ogma3.Data;
using Ogma3.Data.Clubs;
using Ogma3.Data.Folders;
using Ogma3.Data.Stories;
using Ogma3.Infrastructure.Extensions;
using Ogma3.Pages.Shared;
using Ogma3.Pages.Shared.Bars;
using Ogma3.Pages.Shared.Cards;

namespace Ogma3.Pages.Club.Folders
{
    public class FolderModel : PageModel
    {
        private readonly ClubRepository _clubRepo;
        private readonly FoldersRepository _foldersRepo;
        private readonly StoriesRepository _storiesRepo;
        private readonly OgmaConfig _config;

        public FolderModel(ClubRepository clubRepo, FoldersRepository foldersRepo, OgmaConfig config, StoriesRepository storiesRepo)
        {
            _clubRepo = clubRepo;
            _foldersRepo = foldersRepo;
            _config = config;
            _storiesRepo = storiesRepo;
        }
        
        public ClubBar ClubBar { get; set; }
        public FolderDto Folder { get; set; }
        public bool EditPermitted { get; set; }
        public List<StoryCard> Stories { get; set; }
        public Pagination Pagination { get; set; }
        
        public async Task<IActionResult> OnGetAsync(long clubId, long id, [FromQuery] int page = 1)
        {
            var uid = User.GetNumericId();
            
            ClubBar = await _clubRepo.GetClubBar(clubId);
            if (ClubBar == null) return NotFound();

            Folder = await _foldersRepo.GetFolder(id);
            if (Folder == null) return NotFound();

            EditPermitted = await _clubRepo.CheckRoles(Folder.Id, uid, new[]
            {
                EClubMemberRoles.Founder, 
                EClubMemberRoles.Admin
            });
            
            Stories = await _storiesRepo.GetPaginatedCardsOfFolder(id, page, _config.StoriesPerPage);

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