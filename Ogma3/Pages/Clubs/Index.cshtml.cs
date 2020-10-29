using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Ogma3.Data.Enums;
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
        public string? Query { get; set; }
        public EClubSortingOptions SortBy { get; set; }

        public const int PerPage = 10;
        public Pagination Pagination { get; set; }
        
        public async Task OnGetAsync(
            [FromQuery] int page = 1, 
            [FromQuery] string? q = null, 
            [FromQuery] EClubSortingOptions sort = EClubSortingOptions.CreationDateDescending
        ) {
            Query = q;
            SortBy = sort;
            
            Clubs = await _clubRepo.SearchAndSortPaginatedClubCards(page, PerPage, q, sort);

            var count = string.IsNullOrEmpty(q) 
                ? await _clubRepo.CountClubs() 
                : await _clubRepo.CountSearchedClubs(q);

            // Prepare pagination
            Pagination = new Pagination
            {
                PerPage = PerPage,
                ItemCount = count,
                CurrentPage = page
            };
        }
    }
}
