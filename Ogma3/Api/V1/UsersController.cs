using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NSwag.Annotations;
using Ogma3.Data;
using Ogma3.Data.ModeratorActions;
using Ogma3.Data.Users;
using Ogma3.Infrastructure.Comparers;
using Ogma3.Infrastructure.Constants;
using Ogma3.Infrastructure.Extensions;
using Serilog;

namespace Ogma3.Api.V1
{
    [Route("api/[controller]", Name = nameof(UsersController))]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public UsersController(ApplicationDbContext context)
        {
            _context = context;
        }

        // api/Users/block
        [HttpPost("block")]
        public async Task<ActionResult<bool>> BlockUser(BlockPostData data)
        {
            var uid = User.GetNumericId();
            if (uid is null) return Unauthorized();

            var targetUserId = await _context.Users
                .Where(u => u.NormalizedUserName == data.Name.Normalize().ToUpper())
                .Select(u => u.Id)
                .FirstOrDefaultAsync();

            var existing = await _context.BlacklistedUsers
                .Where(bu => bu.BlockingUserId == targetUserId)
                .Where(bu => bu.BlockedUserId == uid)
                .FirstOrDefaultAsync();
            
            if (existing is null)
            {
                _context.BlacklistedUsers.Add(new UserBlock
                {
                    BlockingUserId = targetUserId,
                    BlockedUserId = (long) uid 
                });
                await _context.SaveChangesAsync();
                return true;
            }

            _context.BlacklistedUsers.Remove(existing);
            await _context.SaveChangesAsync();
            return false;
        }

        // api/Users/follow
        [HttpPost("follow")]
        public async Task<ActionResult<bool>> FollowUser(BlockPostData data)
        {
            var uid = User.GetNumericId();
            if (uid is null) return Unauthorized();

            var targetUserId = await _context.Users
                .Where(u => u.NormalizedUserName == data.Name.Normalize().ToUpper())
                .Select(u => u.Id)
                .FirstOrDefaultAsync();

            var existing = await _context.FollowedUsers
                .Where(bu => bu.FollowingUserId == uid)
                .Where(bu => bu.FollowedUserId == targetUserId)
                .FirstOrDefaultAsync();
            
            if (existing is null)
            {
                _context.FollowedUsers.Add(new UserFollow
                {
                    FollowingUserId = (long) uid ,
                    FollowedUserId = targetUserId
                });
                await _context.SaveChangesAsync();
                return true;
            }

            _context.FollowedUsers.Remove(existing);
            await _context.SaveChangesAsync();
            return false;
        }

        [HttpPost("roles")]
        [Authorize(Roles = RoleNames.Admin)]        
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult> ManageRoles(RoleData data)
        {
            var (userId, roles) = data;
            
            // Check if user is logged in
            var uid = User.GetNumericId();
            if (uid is null) return Unauthorized();
            
            var user = await _context.Users
                .Where(u => u.Id == userId)
                .Include(u => u.Roles)
                .FirstOrDefaultAsync();
            
            var newRoles = await _context.OgmaRoles
                .Where(ur => roles.Contains(ur.Id))
                .ToListAsync();

            // Handle role removal
            var removedRoles = user.Roles.Except(newRoles, new OgmaRoleComparer()).ToList();
            foreach (var role in removedRoles)
            {
                user.Roles.Remove(role);
            }
            await _context.ModeratorActions.AddRangeAsync(removedRoles.Select(r => new ModeratorAction
            {
                StaffMemberId = (long)uid,
                Description = ModeratorActionTemplates.UserRoleRemoved(user, User.GetUsername(), r.Name)
            }));
            
            // Handle role adding
            var addedRoles = newRoles.Except(user.Roles, new OgmaRoleComparer()).ToList();
            foreach (var role in addedRoles)
            {
                user.Roles.Add(role);
            }
            await _context.ModeratorActions.AddRangeAsync(addedRoles.Select(r => new ModeratorAction
            {
                StaffMemberId = (long)uid,
                Description = ModeratorActionTemplates.UserRoleAdded(user, User.GetUsername(), r.Name)
            }));

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (Exception e)
            {
                Log.Error(e, "Exception occurred when staff member {Staff} tried adding roles {Role} to user {User}", uid, roles, userId);
                return BadRequest(e.Message);
            }
            return Ok();
        }
        

        // Don't delete or this whole controller will break
        [HttpGet, OpenApiIgnore] public string Ping() => "Pong";
    }

    public sealed record BanData(long UserId, double? Days);
    public sealed record RoleData(long UserId, IEnumerable<long> Roles);
    public sealed record BlockPostData(string Name);
}
