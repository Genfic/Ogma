using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Ogma3.Data.Models;
using Ogma3.Data.Repositories;
using Ogma3.Pages.Shared;

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

        public ICollection<Folder> Folders { get; set; }

        public async Task<IActionResult> OnGetAsync(long clubId, long id)
        {
            ClubBar = await _clubRepo.GetClubBar(clubId);
            if (ClubBar == null) return NotFound();

            Folders = await _foldersRepo.GetTopLevelOfClub(clubId);
            
            return Page();

        }
    }
}