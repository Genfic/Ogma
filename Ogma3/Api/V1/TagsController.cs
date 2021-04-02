using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Ogma3.Data;
using Ogma3.Data.Tags;

namespace Ogma3.Api.V1
{
    [Route("api/[controller]", Name = nameof(TagsController))]
    [ApiController]
    public class TagsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly TagsRepository _tagsRepo;
        
        public TagsController(ApplicationDbContext context, TagsRepository tagsRepo)
        {
            _context = context;
            _tagsRepo = tagsRepo;
        }

        // GET: api/Tags/all
        [HttpGet("all")]
        // TODO: Make access to all tags better for the users
        // [Authorize(Roles = RoleNames.Admin + "," + RoleNames.Moderator)]
        public async Task<ActionResult<IEnumerable<TagDto>>> GetAll()
        {
            return await _tagsRepo.GetAll();
        }

        // GET: api/Tags?page=1&perPage=10
        [HttpGet]
        public async Task<ActionResult<IEnumerable<TagDto>>> GetTags([FromQuery] int page, [FromQuery] int perPage)
        {
            return await _tagsRepo.GetPaginated(page, perPage);
        }


        // GET: api/Tags/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Tag>> GetTag(long id)
        {
            var tag = await _context.Tags
                .AsNoTracking()
                .FirstOrDefaultAsync(t => t.Id == id);

            if (tag == null) return NotFound();

            return tag;
        }

        // GET: api/Tags/story/5
        [HttpGet("story/{id}")]
        public async Task<ActionResult<IEnumerable<TagDto>>> GetStoryTags(long id)
        {
            var tags = await _tagsRepo.GetTagsOfStory(id);

            if (tags == null || tags.Count <= 0) return NotFound();
            
            return tags;
        }
        
        // GET: api/Tags/validation
        [HttpGet("validation")]
        public ActionResult GetTagValidation()
        {
            return Ok(new
            {
                CTConfig.CTag.MinNameLength,
                CTConfig.CTag.MaxNameLength,
                CTConfig.CTag.MaxDescLength,
            });
        }


        // PUT: api/Tags/5
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> PutTag(long id, PostData data)
        {
            var (tagId, name, description, eTagNamespace) = data;
            
            if (id != tagId) return BadRequest();

            var tag = new Tag
            {
                Id = id,
                Name = name,
                Description = description,
                Namespace = eTagNamespace
            };

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
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<Tag>> PostTag(PostData data)
        {
            var (_, name, description, eTagNamespace) = data;
            var tag = new Tag
            {
                Name = name,
                Description = description,
                Namespace = eTagNamespace
            };
            
            await _context.Tags.AddAsync(tag);

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

        private bool TagExists(long id)
        {
            return _context.Tags.Any(e => e.Id == id);
        }

        public sealed record PostData(long? Id, string Name, string? Description, ETagNamespace? Namespace);
    }
}
