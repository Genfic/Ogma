using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Ogma3.Data.Models;

namespace Ogma3.Api.V1
{
    [Route("api/[controller]", Name = nameof(UserActivityController))]
    [ApiController]
    public class UserActivityController : ControllerBase
    {
        private readonly UserManager<OgmaUser> _userManager;

        public UserActivityController(UserManager<OgmaUser> userManager)
        {
            _userManager = userManager;
        }

        // POST
        [HttpHead]
        public async Task<IActionResult> UpdateLastActiveAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return NotFound();
            user.LastActive = DateTime.Now;
            await _userManager.UpdateAsync(user);
            return Ok();
        }
        
        /// <summary>
        /// Plain, parameterless `GET` needs to be here or fuckery happens
        /// </summary>
        [HttpGet] public IActionResult Ping() => Ok("Pong");
    }
}