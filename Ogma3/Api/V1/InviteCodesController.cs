using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Ogma3.Data;
using Ogma3.Data.DTOs;
using Ogma3.Data.Models;

namespace Ogma3.Api.V1
{
    [ApiController]
    [Route("api/[controller]", Name = nameof(InviteCodesController))]
    public class InviteCodesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly OgmaUserManager _userManager;
        private readonly OgmaConfig _config;

        public InviteCodesController(ApplicationDbContext context, OgmaUserManager userManager, OgmaConfig config)
        {
            _context = context;
            _userManager = userManager;
            _config = config;
        }
        
        // GET: api/InviteCodes
        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<IEnumerable<InviteCodeApiDTO>>> GetInviteCodes()
        {
            return await _context.InviteCodes
                .Include(ic => ic.UsedBy)
                .Select(ic => InviteCodeApiDTO.FromInviteCode(ic))
                .AsNoTracking()
                .ToListAsync();
        }
        
        // GET: api/InviteCodes/5
        [HttpGet("{userId}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<IEnumerable<InviteCodeApiDTO>>> GetUserInviteCodes(long userId)
        {
            return await _context.InviteCodes
                .Include(ic => ic.UsedBy)
                .Where(ic => ic.IssuedById == userId)
                .Select(ic => InviteCodeApiDTO.FromInviteCode(ic))
                .AsNoTracking()
                .ToListAsync();
        }
        
        
        // GET: api/InviteCodes/5
        [HttpGet("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<InviteCodeApiDTO>> GetInviteCode(long id)
        {
            var code = await _context.InviteCodes
                .Where(ic => ic.Id == id)
                .Include(ic => ic.UsedBy)
                .AsNoTracking()
                .FirstOrDefaultAsync();

            if (code == null)
            {
                return NotFound();
            }

            return InviteCodeApiDTO.FromInviteCode(code);
        }
        
        
        [HttpPost]
        [Authorize]
        public async Task<ActionResult<InviteCodeApiDTO>> PostInviteCode()
        {
            Console.WriteLine(1);
            var currentUser = await _userManager.GetUserAsync(ClaimsPrincipal.Current);

            Console.WriteLine(2);
            var issuedCount = await _context.InviteCodes
                .CountAsync(ic => ic.IssuedById == currentUser.Id);

            Console.WriteLine(3);
            if (issuedCount > _config.MaxInvitesPerUser) 
                return Unauthorized($"You cannot generate more than {_config.MaxInvitesPerUser} codes");
            
            var code = new InviteCode
            {
                Code = GenerateCode(),
                IssuedBy = currentUser
            };
            await _context.InviteCodes.AddAsync(code);

            await _context.SaveChangesAsync();
            
            return CreatedAtAction("GetInviteCode", new { id = code.Id }, code);
        }


        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<long>> DeleteInviteCode(long id)
        {
            var code = await _context.InviteCodes.FindAsync(id);
            _context.InviteCodes.Remove(code);
            await _context.SaveChangesAsync();
            return id;
        }

        
        private static string GenerateCode()
        {
            var unix = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
            var unixStr = unix.ToString("0000000000").Substring(10);

            var random = new Random();
            var bytes = new byte[5];
            random.NextBytes(bytes);

            var hexArray = Array.ConvertAll(bytes, x => x.ToString("X2"));
            var hexStr = string.Concat(hexArray);
            
            return unixStr + hexStr;
        }
    }
}