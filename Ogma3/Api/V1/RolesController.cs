using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Ogma3.Data;
using Ogma3.Data.AuthorizationData;
using Ogma3.Data.Roles;

namespace Ogma3.Api.V1
{
    [Route("api/[controller]", Name = nameof(RolesController))]
    [ApiController]
    public class RolesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly RoleManager<OgmaRole> _roleManager;

        public RolesController(ApplicationDbContext context, RoleManager<OgmaRole> roleManager)
        {
            _context = context;
            _roleManager = roleManager;
        }


        // GET: api/Roles
        [HttpGet]
        public async Task<ActionResult<IEnumerable<OgmaRole>>> GetRoles()
        {
            return await _context.Roles
                .OrderByDescending(ns => ns.Order.HasValue)
                    .ThenByDescending(ns => ns.Order)
                .AsNoTracking()
                .ToListAsync();
        }

        // PUT: api/Namespaces/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://aka.ms/RazorPagesCRUD.
        [HttpPut("{id}")]
        [Authorize(Roles = RoleNames.Admin)]
        public async Task<ActionResult<OgmaRole>> PutRole(long id, PostData data)
        {
            if (data.Id == null) return NotFound();
            if (id != data.Id) return BadRequest();

            var role = await _roleManager.FindByIdAsync(id.ToString());

            role.Name = data.Name;
            role.IsStaff = data.IsStaff;
            role.Color = data.Color;
            role.Order = data.Order;

            try
            {
                await _roleManager.UpdateAsync(role);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
            return CreatedAtAction("GetRoles", role);
        }


        // POST: api/Roles
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://aka.ms/RazorPagesCRUD.
        [HttpPost]
        [Authorize(Roles = RoleNames.Admin)]
        public async Task<ActionResult<OgmaRole>> PostRole(OgmaRole data)
        {
            if (await _roleManager.RoleExistsAsync(data.Name))
            {
                return Conflict($"A role with name {data.Name} already exists!");
            }

            var role = new OgmaRole
            {
                Name = data.Name,
                IsStaff = data.IsStaff,
                Color = data.Color,
                Order = data.Order
            };

            try
            {
                await _roleManager.CreateAsync(role);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
            return CreatedAtAction("GetRoles", role);
        }


        // DELETE: api/Roles/5
        [HttpDelete("{id}")]
        [Authorize(Roles = RoleNames.Admin)]
        public async Task<IActionResult> DeleteRole(long id)
        {
            var role = await _roleManager.FindByIdAsync(id.ToString());
            try
            {
                await _roleManager.DeleteAsync(role);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
            return Ok();
        }

        public class PostData
        {
            public long? Id { get; set; }
            public string Name { get; set; }
            public string? Color { get; set; }
            public bool IsStaff { get; set; }
            public byte? Order { get; set; }
        }
    }
}
