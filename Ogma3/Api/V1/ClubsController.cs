using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Ogma3.Data;
using Ogma3.Data.Clubs;
using Ogma3.Infrastructure.Constants;
using Ogma3.Infrastructure.Extensions;

namespace Ogma3.Api.V1
{
    [Route("api/[controller]", Name = nameof(ClubsController))]
    [ApiController]
    public class ClubsController : ControllerBase
    {
        private readonly ClubRepository _clubRepo;
        private readonly ApplicationDbContext _context;

        public ClubsController(ClubRepository clubRepo, ApplicationDbContext context)
        {
            _clubRepo = clubRepo;
            _context = context;
        }

        // GET: /api/clubs/user
        [HttpGet("user")]
        [Authorize]
        public async Task<ActionResult<List<UserClubMinimalDto>>> GetUserClubs()
        {
            var uid = User.GetNumericId();
            if (uid is null) return Unauthorized();

            return await _context.Clubs
                .Where(c => c.ClubMembers.Any(cm => cm.MemberId == (long)uid))
                .OrderBy(c => c.Name)
                .Select(c => new UserClubMinimalDto(c.Id, c.Name, c.Icon))
                .AsNoTracking()
                .ToListAsync();
        }
        public record UserClubMinimalDto(long Id, string Name, string Icon);

        // GET: /api/clubs/story/3
        [HttpGet("story/{id:long}")]
        public async Task<ActionResult<List<ClubMinimalDto>>> GetClubsWithStory(long id)
        {
            return await _context.Clubs
                .Where(c => c.Folders
                    .Any(f => f.Stories
                        .Any(s => s.Id == id)
                    )
                )
                .Select(c => new ClubMinimalDto
                {
                    Id = c.Id,
                    Name = c.Name,
                    Icon = c.Icon
                })
                .ToListAsync();
        }

        [HttpDelete]
        [Authorize]
        public async Task<ActionResult<bool>> DeleteTopicAsync(long id)
        {
            var uid = User.GetNumericId();
            if (uid is null) return Unauthorized();

            bool canDelete;
            if (User.IsInRole(RoleNames.Admin) || User.IsInRole(RoleNames.Moderator))
            {
                canDelete = true;
            }
            else
            {
                var roles = new[] { EClubMemberRoles.Founder, EClubMemberRoles.Admin, EClubMemberRoles.Moderator };

                canDelete = await _context.ClubThreads
                    .Where(ct => ct.Id == id)
                    .Select(ct => ct.Club.ClubMembers
                        .Where(cm => cm.MemberId == uid)
                        .Any(cm => roles.Contains(cm.Role)))
                    .FirstOrDefaultAsync();
            }

            if (!canDelete) return Unauthorized();

            var topic = await _context.ClubThreads.FindAsync(id);
            topic.DeletedAt = DateTime.Now;
            var res = await _context.SaveChangesAsync();

            return res > 0;
        }

        // Don't delete or this whole controller will break
        [HttpGet]
        public string Ping() => "Pong";
    }
}