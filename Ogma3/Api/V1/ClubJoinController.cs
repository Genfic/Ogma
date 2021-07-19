using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Ogma3.Data;
using Ogma3.Data.Clubs;
using Serilog;

namespace Ogma3.Api.V1
{
    [Route("api/[controller]", Name = nameof(ClubJoinController))]
    [ApiController]
    public class ClubJoinController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public ClubJoinController(ApplicationDbContext context)
        {
            _context = context;
        }
        
        // GET api/clubjoin/5
        [HttpGet("{club:long}")]
        [Authorize]
        public async Task<ActionResult<bool>> GetClubMember(long club)
        {
            var uid = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (uid is null) return NotFound();
            
            var isMember = await _context.ClubMembers
                .AnyAsync(cm => cm.ClubId == club && cm.MemberId == Convert.ToInt64(uid));
            
            return new OkObjectResult(isMember);
        }
        
        // POST api/clubjoin
        [HttpPost]
        [Authorize]
        [IgnoreAntiforgeryToken]
        public async Task<ActionResult<bool>> PostClubMember(PostModel data)
        {
            var uid = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (uid is null) return NotFound();

            var nUid = Convert.ToInt64(uid);
            
            var clubMember = await _context.ClubMembers
                .Where(cm => cm.ClubId == data.ClubId && cm.MemberId == nUid)
                .AsNoTracking()
                .FirstOrDefaultAsync();

            var member = new ClubMember
            {
                ClubId = data.ClubId,
                MemberId = nUid,
                Role = EClubMemberRoles.User,
                MemberSince = DateTime.Now
            };
            
            // If no such member exists, add one
            bool isMember;
            if (clubMember is null)
            {
                _context.ClubMembers.Add(member);
                isMember = true;
            }
            else // If the member does exist, remove them
            {
                // But only if it's not the founder
                if (clubMember.Role != EClubMemberRoles.Founder)
                {
                    _context.ClubMembers.Remove(clubMember);
                    isMember = false;
                }
                else
                {
                    isMember = true;
                }
            }

            // Save
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (Exception e)
            {
                Log.Error(e, "Exception occurred when user {User} tried joining club {Club}", uid, data.ClubId);
                return StatusCode(500);
            }

            return new OkObjectResult(isMember);
        }
        
        public sealed record PostModel(long ClubId);

    }
}