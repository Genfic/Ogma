using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Ogma3.Data;
using Utils;

namespace Ogma3.Api
{
    [Route("api/[controller]", Name = "UsersController")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly OgmaUserManager _userManager;

        public UsersController(OgmaUserManager userManager)
        {
            _userManager = userManager;
        }

        [HttpGet("signin/{name}")]
        public async Task<ActionResult<SignInData>> GetSignInData(string name)
        {
            var user = await _userManager.FindByNameAsync(name);

            if (user != null)
            {
                return new SignInData
                {
                    Avatar = user.Avatar ?? Lorem.Picsum(200),
                    Title = user.Title,
                    HasMfa = user.TwoFactorEnabled
                };
            }

            return NoContent();
        }
    }

    public class SignInData
    {
        public string Avatar { get; set; }
        public string Title { get; set; }
        public bool HasMfa { get; set; }
    }
}
