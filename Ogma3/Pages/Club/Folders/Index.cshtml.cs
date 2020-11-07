using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Ogma3.Data.Models;
using Ogma3.Data.Repositories;
using Ogma3.Pages.Shared;
using Ogma3.Pages.Shared.Bars;
using Ogma3.Pages.Shared.Cards;

namespace Ogma3.Pages.Club.Folders
{
    public class IndexModel : PageModel
    {
        private readonly ClubRepository _clubRepo;
        private readonly FoldersRepository _foldersRepo;

        public IndexModel(ClubRepository clubRepo, FoldersRepository foldersRepo)
        {
            _clubRepo = clubRepo;
            _foldersRepo = foldersRepo;
        }
        
        public ClubBar ClubBar { get; set; }

        public ICollection<FolderCard> Folders { get; set; }

        public async Task<IActionResult> OnGetAsync(long id)
        {
            ClubBar = await _clubRepo.GetClubBar(id);
            if (ClubBar == null) return NotFound();

            Folders = await _foldersRepo.GetClubFolderCards(id);
            
            return Page();

        }
    }
}