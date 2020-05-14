using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Ogma3.Data;
using Ogma3.Data.Models;

namespace Ogma3.Api
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
        public async Task<ActionResult<IEnumerable<InviteCode>>> GetInviteCodes()
        {
            return await _context.InviteCodes.ToListAsync();
        }
        
        
        // GET: api/InviteCodes/5
        [HttpGet("{id}")]
        public async Task<ActionResult<InviteCode>> GetInviteCode(long id)
        {
            var code = await _context.InviteCodes.FindAsync(id);

            if (code == null)
            {
                return NotFound();
            }

            return code;
        }
        
        
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<InviteCode>> PostInviteCode()
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