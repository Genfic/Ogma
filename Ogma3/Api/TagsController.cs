using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Server.Kestrel.Transport.Libuv.Internal;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Ogma3.Data;
using Ogma3.Data.DTOs;
using Ogma3.Data.Models;

namespace Ogma3.Api
{
    [Route("api/[controller]", Name = nameof(TagsController))]
    [ApiController]
    public class TagsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public TagsController(ApplicationDbContext context)
        {
            _context = context;
        }


        // GET: api/Tags
        [HttpGet]
        public async Task<ActionResult<IEnumerable<TagDTO>>> GetTags()
        {
            var tags = await _context.Tags
                .Include(t => t.Namespace)
                .ToListAsync();
            return tags.Select(TagDTO.FromTag).ToList();
        }


        // GET: api/Tags/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Tag>> GetTag(int id)
        {
            var tag = await _context.Tags.FindAsync(id);

            if (tag == null)
            {
                return NotFound();
            }

            return tag;
        }
        
        // GET: api/Tags/validation
        [HttpGet("validation")]
        public ActionResult GetTagValidation()
        {
            return Ok(new
            {
                CTConfig.Tag.MinNameLength,
                CTConfig.Tag.MaxNameLength,
                CTConfig.Tag.MaxDescLength,
            });
        }


        // PUT: api/Tags/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://aka.ms/RazorPagesCRUD.
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> PutTag(int id, Tag tag)
        {
            if (id != tag.Id)
            {
                return BadRequest();
            }

            _context.Entry(tag).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TagExists(id))
                {
                    return NotFound();
                }

                throw;
            }
            catch (DbUpdateException ex) when (ex.InnerException is SqlException sqlException && (sqlException.Number == 2627 || sqlException.Number == 2601))
            {
                return Conflict(new { message = $"A tag with the name '{tag.Name}' already exists" });
            }

            return NoContent();
        }


        // POST: api/Tags
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://aka.ms/RazorPagesCRUD.
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<Tag>> PostTag(Tag tag)
        {
            _context.Tags.Add(tag);

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException ex) when (ex.InnerException is SqlException sqlException && (sqlException.Number == 2627 || sqlException.Number == 2601))
            {
                return Conflict(new { message = $"A tag with the name '{tag.Name}' already exists" });
            }

            return CreatedAtAction("GetTag", new { id = tag.Id }, tag);
        }


        // DELETE: api/Tags/5
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<Tag>> DeleteTag(int id)
        {
            var tag = await _context.Tags.FindAsync(id);
            if (tag == null)
            {
                return NotFound();
            }

            _context.Tags.Remove(tag);
            await _context.SaveChangesAsync();

            return tag;
        }

        private bool TagExists(int id)
        {
            return _context.Tags.Any(e => e.Id == id);
        }
    }
}
