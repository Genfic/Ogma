using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Server.Kestrel.Internal.System.IO.Pipelines;
using Microsoft.EntityFrameworkCore;
using Ogma3.Data;
using Ogma3.Data.ClubModeratorActions;
using Ogma3.Data.Clubs;
using Ogma3.Data.ModeratorActions;
using Ogma3.Infrastructure.Constants;
using Ogma3.Infrastructure.Extensions;
using Serilog;

namespace Ogma3.Api.V1
{
    [Route("api/[controller]", Name = nameof(CommentsThreadController))]
    [ApiController]
    public class CommentsThreadController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        
        public CommentsThreadController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet("permissions/{id:long}")]
        public async Task<PermissionsResult> GetPermissionsAsync(long id)
        {
            if (User.Identity is { IsAuthenticated: false }) return new PermissionsResult(false, false);

            var uid = User.GetNumericId();
            if (uid is null) return new PermissionsResult(false, false);

            var isSiteModerator = User.IsInRole(RoleNames.Admin) || User.IsInRole(RoleNames.Moderator);
            
            var roles = new[] { EClubMemberRoles.Founder, EClubMemberRoles.Admin, EClubMemberRoles.Moderator };
            var isClubModerator = await _context.CommentThreads
                .Where(ct => ct.Id == id)
                .Where(ct => ct.ClubThreadId != null)
                .Select(ct => ct.ClubThread.Club.ClubMembers
                    .Where(cm => cm.MemberId == uid)
                    .Any(cm => roles.Contains(cm.Role)))
                .FirstOrDefaultAsync();
            
            return new PermissionsResult(isSiteModerator, isClubModerator);
        }
        public sealed record PermissionsResult(bool IsSiteModerator, bool IsClubModerator)
        {
            public bool IsAllowed => IsSiteModerator || IsClubModerator;
        }

        
        [HttpGet("lock/status/{id:long}")]
        public async Task<bool> GetLockStatusAsync(long id) 
            => await _context.CommentThreads
                .Where(ct => ct.Id == id)
                .Select(ct => ct.LockDate != null)
                .FirstOrDefaultAsync();

        // POST
        [HttpPost("lock")]
        [Authorize]
        [IgnoreAntiforgeryToken]
        public async Task<IActionResult> LockThreadAsync([FromBody]PostData data)
        {
            var permission = await GetPermissionsAsync(data.Id);
            if (!permission.IsAllowed) return Unauthorized();
            
            var thread = await _context.CommentThreads.FindAsync(data.Id);
            thread.LockDate = thread.LockDate is null ? DateTime.Now : null;

            if (User.IsInRole(RoleNames.Admin) || User.IsInRole(RoleNames.Moderator))
            {
                var uid = User.GetNumericId();
                if (uid is null) return Unauthorized();

                string type;
                if (thread.BlogpostId is not null) type = "blogpost";
                else if (thread.ChapterId is not null) type = "chapter";
                else if (thread.ClubThreadId is not null) type = "club";
                else if (thread.UserId is not null) type = "user profile";
                else type = "unknown";

                var typeId = thread.BlogpostId ?? thread.ChapterId ?? thread.ClubThreadId ?? thread.UserId ?? 0;

                if (permission.IsSiteModerator && !permission.IsClubModerator)
                {
                    _context.ModeratorActions.Add(new ModeratorAction
                    {
                        StaffMemberId = (long) uid,
                        Description = thread.LockDate is null
                            ? ModeratorActionTemplates.ThreadUnlocked(type, typeId, thread.Id, User.GetUsername())
                            : ModeratorActionTemplates.ThreadLocked(type, typeId, thread.Id, User.GetUsername())
                    });
                }
                else if (permission.IsClubModerator && thread.ClubThreadId is not null)
                {
                    var clubId = await _context.ClubThreads
                        .Where(ct => ct.CommentsThread.Id == data.Id)
                        .Select(ct => ct.ClubId)
                        .FirstOrDefaultAsync();
                    _context.ClubModeratorActions.Add(new ClubModeratorAction
                    {
                        ModeratorId = (long)uid,
                        ClubId = clubId,
                        Description = thread.LockDate is null
                            ? ModeratorActionTemplates.ThreadUnlocked(type, typeId, thread.Id, User.GetUsername())
                            : ModeratorActionTemplates.ThreadLocked(type, typeId, thread.Id, User.GetUsername())
                    });
                }
                else
                {
                    Log.Error("Comment thread was locked in an unexpected way. Permission {0}", permission);
                }
            }

            await _context.SaveChangesAsync();
                
            return Ok(thread.LockDate is not null);
        }

        
        // Don't delete or this whole controller will break
        [HttpGet] public string Ping() => "Pong";

        public sealed record PostData(long Id);
    }
}