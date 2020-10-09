using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Ogma3.Data;
using Utils.Extensions;

namespace Ogma3.Api.V1
{
    [Route("api/[controller]", Name = nameof(UserActivityController))]
    [ApiController]
    public class UserActivityController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public UserActivityController(ApplicationDbContext context)
        {
            _context = context;
        }

        // POST
        [HttpHead]
        public async Task<IActionResult> UpdateLastActiveAsync()
        {
            var uid = User.GetNumericId();
            await _context.Database.ExecuteSqlInterpolatedAsync(
                $"UPDATE \"AspNetUsers\" SET \"LastActive\" = {DateTime.Now.ToUniversalTime()} WHERE \"Id\" = {uid}"
            );
            return Ok();
        }
        
        /// <summary>
        /// Plain, parameterless `GET` needs to be here or fuckery happens
        /// </summary>
        [HttpGet] public IActionResult Ping() => Ok("Pong");
    }
}