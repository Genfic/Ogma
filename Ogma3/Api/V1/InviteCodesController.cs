using System;
using System.Collections.Generic;
using System.Linq;
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

        public InviteCodesController(ApplicationDbContext context)
        {
            _context = context;
        }
        
        // GET: api/InviteCodes
        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<IEnumerable<InviteCodeApiDTO>>> GetInviteCodes()
        {
            return await _context.InviteCodes
                .Include(ic => ic.UsedBy)
                .Select(ic => InviteCodeApiDTO.FromInviteCode(ic))
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
                .FirstOrDefaultAsync();

            if (code == null)
            {
                return NotFound();
            }

            return InviteCodeApiDTO.FromInviteCode(code);
        }
        
        
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<InviteCodeApiDTO>> PostInviteCode()
        {
            var code = new InviteCode {Code = GenerateCode()};
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