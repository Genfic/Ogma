using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Ogma3.Data;
using Ogma3.Data.Models;

namespace Ogma3.Api.V1
{
    [Route("api/[controller]", Name = nameof(ClubJoinController))]
    [ApiController]
    public class ClubJoinController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly OgmaUserManager _userManager;

        public ClubJoinController(OgmaUserManager userManager, ApplicationDbContext context)
        {
            _userManager = userManager;
            _context = context;
        }
        
        // GET api/clubjoin/5
        [HttpGet("{club}")]
        [Authorize]
        public async Task<ActionResult<bool>> GetClubMember(long club)
        {
            // var user = await _userManager.GetUserAsync(User);
            var uid = long.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
            
            var isMember = await _context.ClubMembers
                .AnyAsync(cm => cm.ClubId == club && cm.MemberId == uid);
            
            return new OkObjectResult(isMember);
        }
        
        // POST api/chaptersread
        [HttpPost]
        [Authorize]
        public async Task<ActionResult<bool>> PostClubMember(PostModel data)
        {
            var uid = long.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
            var clubMember = await _context.ClubMembers
                .Where(cm => cm.ClubId == data.ClubId && cm.MemberId == uid)
                .AsNoTracking()
                .FirstOrDefaultAsync();

            var member = new ClubMember
            {
                ClubId = data.ClubId,
                MemberId = uid
            };
            
            // If no such member exists, add one
            bool isMember;
            if (clubMember == null)
            {
                await _context.ClubMembers.AddAsync(member);
                isMember = true;
            }
            else // If the member does exist, remove them
            {
                _context.ClubMembers.Remove(member);
                isMember = false;
            }

            // Save
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return StatusCode(500);
            }

            return new OkObjectResult(isMember);
        }

        public sealed class PostModel
        {
            public long ClubId { get; set; }
        }
    }
}