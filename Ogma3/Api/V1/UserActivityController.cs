using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Ogma3.Data.Models;

namespace Ogma3.Api.V1
{
    [Route("api/[controller]", Name = nameof(UserActivityController))]
    public class UserActivityController : ControllerBase
    {
        private readonly UserManager<OgmaUser> _userManager;

        public UserActivityController(UserManager<OgmaUser> userManager)
        {
            _userManager = userManager;
        }

        // POST
        [HttpPost]
        public async Task UpdateLastActiveAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return;
            user.LastActive = DateTime.Now;
            await _userManager.UpdateAsync(user);
        }
    }
}