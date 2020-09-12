using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Ogma3.Data;
using Ogma3.Data.Models;
using Ogma3.Data.Repositories;
using Ogma3.Pages.Shared;

namespace Ogma3.Pages.Club.Forums
{
    public class DetailsModel : PageModel
    {
        private readonly ClubRepository _clubRepo;
        private readonly ThreadRepository _threadRepo;

        public DetailsModel(ClubRepository clubRepo, ThreadRepository threadRepo)
        {
            _clubRepo = clubRepo;
            _threadRepo = threadRepo;
        }

        public ThreadDetails ClubThread { get; set; }
        public ClubBar ClubBar { get; set; }

        public async Task<IActionResult> OnGetAsync(long clubId, long threadId)
        {
            var uid = long.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
            ClubBar = await _clubRepo.GetClubBar(clubId, uid);

            if (ClubBar == null)
            {
                return NotFound();
            }

            ClubThread = await _threadRepo.GetThreadDetails(threadId);

            if (ClubThread == null)
            {
                return NotFound();
            }
            return Page();
        }
    }
}
