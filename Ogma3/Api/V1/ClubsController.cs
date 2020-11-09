using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Ogma3.Data.DTOs;
using Ogma3.Data.Repositories;
using Utils.Extensions;

namespace Ogma3.Api.V1
{
    
    [Route("api/[controller]", Name = nameof(ClubsController))]
    [ApiController]
    public class ClubsController : ControllerBase
    {
        private readonly ClubRepository _clubRepo;

        public ClubsController(ClubRepository clubRepo)
        {
            _clubRepo = clubRepo;
        }

        // GET: /api/clubs/user
        [HttpGet("user")]
        [Authorize]
        public async Task<ActionResult<List<UserClubMinimalDto>>> GetUserClubs()
        {
            var uid = User.GetNumericId();
            if (uid == null) return Unauthorized();
            return await _clubRepo.GetUserClubsMinimal((long) uid);
        }
        
        
    }
}