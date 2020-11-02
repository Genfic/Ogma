using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Ogma3.Data.Models;
using Ogma3.Data.Repositories;
using Ogma3.Pages.Shared;

namespace Ogma3.Pages.Club.Folders
{
    public class FolderModel : PageModel
    {
        private readonly ClubRepository _clubRepo;
        private readonly FoldersRepository _foldersRepo;

        public FolderModel(ClubRepository clubRepo, FoldersRepository foldersRepo)
        {
            _clubRepo = clubRepo;
            _foldersRepo = foldersRepo;
        }
        
        public ClubBar ClubBar { get; set; }

        public Folder Folder { get; set; }
        
        public async Task<IActionResult> OnGetAsync(long clubId, long id)
        {
            ClubBar = await _clubRepo.GetClubBar(clubId);
            if (ClubBar == null) return NotFound();

            Folder = await _foldersRepo.GetFolder(id);
            
            return Page();

        }
    }
}