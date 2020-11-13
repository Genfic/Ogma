using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Ogma3.Data;
using Ogma3.Data.Models;
using Utils;
using Utils.Extensions;

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

        // GET: api/Users/signin/John
        [HttpGet("signin/{name}")]
        public async Task<ActionResult<SignInData>> GetSignInData(string name)
        {
            var user = await _context.Users
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.NormalizedUserName == name.ToUpper());

            if (user != null)
            {
                return new SignInData
                {
                    Avatar = user.Avatar ?? Lorem.Picsum(200),
                    Title  = user.Title,
                    HasMfa = user.TwoFactorEnabled
                };
            }

            return NoContent();
        }

        // api/Users/block
        [HttpPost("block")]
        public async Task<ActionResult<bool>> BlockUser(BlockPostData data)
        {
            var uid = User.GetNumericId();
            if (!uid.HasValue) return Unauthorized();

            var targetUserId = await _context.Users
                .Where(u => u.NormalizedUserName == data.Name.Normalize().ToUpper())
                .Select(u => u.Id)
                .FirstOrDefaultAsync();

            var existing = await _context.BlacklistedUsers
                .Where(bu => bu.BlockingUserId == targetUserId)
                .Where(bu => bu.BlockedUserId == uid)
                .FirstOrDefaultAsync();
            
            if (existing == null)
            {
                await _context.BlacklistedUsers.AddAsync(new UserBlock
                {
                    BlockingUserId = targetUserId,
                    BlockedUserId = (long) uid 
                });
                await _context.SaveChangesAsync();
                return true;
            }
            else
            {
                _context.BlacklistedUsers.Remove(existing);
                await _context.SaveChangesAsync();
                return false;
            }
        }
        
        /// <summary>
        /// Plain, parameterless `GET` needs to be here or fuckery happens
        /// </summary>
        [HttpGet] public IActionResult Ping() => Ok("Pong");
    }

    public class SignInData
    {
        public string Avatar { get; set; }
        public string Title { get; set; }
        public bool HasMfa { get; set; }
    }

    public class BlockPostData
    {
        public string Name { get; set; }
    }
}
