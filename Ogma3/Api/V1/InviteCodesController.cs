using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text.Json;
using System.Threading.Tasks;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Ogma3.Data;
using Ogma3.Data.AuthorizationData;
using Ogma3.Data.DTOs;
using Ogma3.Data.Models;
using Utils.Extensions;

namespace Ogma3.Api.V1
{
    [ApiController]
    [Route("api/[controller]", Name = nameof(InviteCodesController))]
    public class InviteCodesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly OgmaUserManager _userManager;
        private readonly OgmaConfig _config;
        private readonly IMapper _mapper;

        public InviteCodesController(ApplicationDbContext context, OgmaUserManager userManager, OgmaConfig config, IMapper mapper)
        {
            _context = context;
            _userManager = userManager;
            _config = config;
            _mapper = mapper;
        }
        
        // GET: api/InviteCodes
        [HttpGet]
        [Authorize]
        public async Task<ActionResult<IEnumerable<InviteCodeApiDto>>> GetInviteCodes()
        {
            var query = _context.InviteCodes
                .Include(ic => ic.UsedBy)
                .AsNoTracking();

            if (!User.HasClaim(RoleClaimTypes.Permission, RoleClaimNames.GetAllInviteCodes))
            {
                query = query.Where(ic => ic.IssuedById == User.GetNumericId());
            }
            
            return await query
                .ProjectTo<InviteCodeApiDto>(_mapper.ConfigurationProvider)
                .ToListAsync();
        }
        
        // GET: api/InviteCodes/all
        [HttpGet("all")]
        [Authorize(Roles = RoleNames.Admin+", "+RoleNames.Moderator)]
        public async Task<ActionResult<IEnumerable<InviteCodeApiDto>>> GetAllInviteCodes()
        {
            var query = _context.InviteCodes
                .Include(ic => ic.UsedBy)
                .AsNoTracking();

            Console.WriteLine(JsonSerializer.Serialize(
                User.Claims.Select(c => new { c.Type, c.Value }), 
                new JsonSerializerOptions{WriteIndented = true})
            );
            
            return await query
                .ProjectTo<InviteCodeApiDto>(_mapper.ConfigurationProvider)
                .ToListAsync();
        }
        
        
        // GET: api/InviteCodes/5
        [HttpGet("{id}")]
        [Authorize]
        public async Task<ActionResult<InviteCodeApiDto>> GetInviteCode(long id)
        {
            var code = await _context.InviteCodes
                .Where(ic => ic.Id == id)
                .Include(ic => ic.UsedBy)
                .ProjectTo<InviteCodeApiDto>(_mapper.ConfigurationProvider)
                .AsNoTracking()
                .FirstOrDefaultAsync();

            if (code == null)
            {
                return NotFound();
            }

            return code;
        }
        
        
        [HttpPost]
        [Authorize]
        public async Task<ActionResult<InviteCodeApiDto>> PostInviteCode()
        {
            var currentUser = await _userManager.GetUserAsync(User);

            var issuedCount = await _context.InviteCodes
                .CountAsync(ic => ic.IssuedById == currentUser.Id);
            
            if (issuedCount >= _config.MaxInvitesPerUser) 
                return Unauthorized($"You cannot generate more than {_config.MaxInvitesPerUser} codes");
            
            var code = new InviteCode
            {
                Code = GenerateCode(),
                IssuedBy = currentUser
            };
            await _context.InviteCodes.AddAsync(code);

            await _context.SaveChangesAsync();
            
            return _mapper.Map<InviteCode, InviteCodeApiDto>(code);
        }
        
        
        [HttpPost("no-limit")]
        [Authorize(Roles = RoleNames.Admin+", "+RoleNames.Moderator)]
        public async Task<ActionResult<InviteCodeApiDto>> PostInviteCodeNoLimit()
        {
            var currentUser = await _userManager.GetUserAsync(User);
            
            var code = new InviteCode
            {
                Code = GenerateCode(),
                IssuedBy = currentUser
            };
            await _context.InviteCodes.AddAsync(code);

            await _context.SaveChangesAsync();
            
            return _mapper.Map<InviteCode, InviteCodeApiDto>(code);
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