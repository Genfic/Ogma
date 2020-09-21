using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Ogma3.Data;
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
        public ICollection<UserSimpleDto> ClubMembers { get; set; }
        
        public async Task<IActionResult> OnGetAsync(long id)
        {
            ClubBar = await _clubRepo.GetClubBar(id, User.GetNumericId());
            
            if (ClubBar == null) return NotFound();

            ClubMembers = await _clubRepo.GetMembers(id, 1, 100);

            return Page();

        }
    }
}