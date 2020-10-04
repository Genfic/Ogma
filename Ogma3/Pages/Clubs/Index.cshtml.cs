using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Ogma3.Data.Repositories;
using Ogma3.Pages.Shared;

namespace Ogma3.Pages.Clubs
{
    public class IndexModel : PageModel
    {
        private readonly ClubRepository _clubRepo;

        public IndexModel(ClubRepository clubRepo)
        {
            _clubRepo = clubRepo;
        }

        public IList<ClubCard> Clubs { get;set; }

        public const int PerPage = 10;
        public Pagination Pagination { get; set; }
        
        public async Task OnGetAsync([FromQuery] int page = 1)
        {
            Clubs = await _clubRepo.GetPaginatedClubCards(page, PerPage);
            
            // Prepare pagination
            Pagination = new Pagination
            {
                PerPage = PerPage,
                ItemCount = await _clubRepo.CountClubs(),
                CurrentPage = page
            };
        }
    }
}
