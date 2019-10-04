using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Ogma3.Data;
using Ogma3.Data.Models;

namespace Ogma3.Api
{
    [Route("api/[controller]", Name = "NamespacesController")]
    [ApiController]
    public class NamespacesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public NamespacesController(ApplicationDbContext context)
        {
            _context = context;
        }


        // GET: api/Namespaces
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Namespace>>> GetNamespace()
        {
            return await _context.Namespaces.ToListAsync();
        }


        // GET: api/Namespaces/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Namespace>> GetNamespace(int id)
        {
            var ns = await _context.Namespaces.FindAsync(id);

            if (ns == null)
            {
                return NotFound();
            }

            return ns;
        }


        // PUT: api/Namespaces/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://aka.ms/RazorPagesCRUD.
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> PutNamespace(int id, Namespace ns)
        {
            if (id != ns.Id)
            {
                return BadRequest();
            }

            _context.Entry(ns).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!NamespaceExists(id))
                {
                    return NotFound();
                }

                throw;
            }
            catch (DbUpdateException ex) when (ex.InnerException is SqlException sqlException && (sqlException.Number == 2627 || sqlException.Number == 2601))
            {
                return Conflict(new { message = $"A ns with the name '{ns.Name}' already exists" });
            }

            return NoContent();
        }


        // POST: api/Namespaces
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://aka.ms/RazorPagesCRUD.
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<Namespace>> PostNamespace(Namespace ns)
        {
            _context.Namespaces.Add(ns);

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException ex) when (ex.InnerException is SqlException sqlException && (sqlException.Number == 2627 || sqlException.Number == 2601))
            {
                return Conflict(new { message = $"A ns with the name '{ns.Name}' already exists" });
            }

            return CreatedAtAction("GetNamespace", new { id = ns.Id }, ns);
        }


        // DELETE: api/Namespaces/5
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<Namespace>> DeleteNamespace(int id)
        {
            var ns = await _context.Namespaces.FindAsync(id);
            if (ns == null)
            {
                return NotFound();
            }

            _context.Namespaces.Remove(ns);
            await _context.SaveChangesAsync();

            return ns;
        }

        private bool NamespaceExists(int id)
        {
            return _context.Namespaces.Any(e => e.Id == id);
        }
    }
}
