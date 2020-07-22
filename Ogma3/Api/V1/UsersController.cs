using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Ogma3.Data;
using Utils;

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
}
