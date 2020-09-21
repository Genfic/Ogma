using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Ogma3.Data.DTOs;
using Ogma3.Data.Repositories;
using Ogma3.Pages.Shared;
using Utils.Extensions;

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
            ClubBar = await _clubRepo.GetClubBar(id, User.GetNumericId());
            
            if (ClubBar == null) return NotFound();

            ClubMembers = await _clubRepo.GetMembers(id, 1, 100);

            return Page();

        }
    }
}