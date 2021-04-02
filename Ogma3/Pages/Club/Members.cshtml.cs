using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Ogma3.Data.Clubs;
using Ogma3.Pages.Shared.Bars;
using Ogma3.Pages.Shared.Cards;

namespace Ogma3.Pages.Club
{
    public class Members : PageModel
    {
        private readonly ClubRepository _clubRepo;

        public Members(ClubRepository clubRepo)
        {
            _clubRepo = clubRepo;
        }


        public ClubBar ClubBar { get; set; }
        public List<UserCard> ClubMembers { get; set; }
        
        public async Task<IActionResult> OnGetAsync(long id)
        {
            ClubBar = await _clubRepo.GetClubBar(id);
            
            if (ClubBar == null) return NotFound();

            ClubMembers = await _clubRepo.GetMembers(id, 1, 100);

            return Page();

        }
    }
}