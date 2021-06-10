using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Ogma3.Data;
using Ogma3.Data.Shelfs;
using Ogma3.Infrastructure.Extensions;
using Serilog;

namespace Ogma3.Api.V1
{
    [Route("api/[controller]", Name = nameof(ShelvesController))]
    [ApiController]
    public class ShelvesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        
        public ShelvesController(ApplicationDbContext context)
        {
            _context = context;
        }
        

        /// <summary>
        /// Get all shelves that belong to user of `name`
        /// </summary>
        /// <param name="name">Name of the shelf owner</param>
        /// <returns>List of `ShelfDto` objects</returns>
        // GET: api/Shelves/JohnSmith
        [HttpGet("user/{name}")]
        public async Task<ActionResult<IEnumerable<ShelfDto>>> GetUserShelvesAsync(string name)
        {
            var uid = User?.GetNumericId();
            
            var shelves = await _context.Shelves
                .Where(s => s.Owner.NormalizedUserName == name.Normalize().ToUpperInvariant())
                .Where(s => s.OwnerId == uid || s.IsPublic)
                .Select(ShelfMappings.ToShelfDto())
                .ToListAsync();
            
            return shelves.Count > 0 ? Ok(shelves) : NoContent();
        }        
        
        /// <summary>
        /// Get all shelves that belong to the current user and check if they contain a `story`
        /// </summary>
        /// <param name="story">Story to check for</param>
        /// <returns></returns>
        [HttpGet("user/{story:long}")]
        public async Task<ActionResult<IEnumerable<ShelfDto>>> GetCurrentUserShelvesAsync(long story)
        {
            var uid = User?.GetNumericId();
            if (uid is null) return Unauthorized();

            var shelves = await _context.Shelves
                .Where(s => s.Owner.Id == uid)
                .Select(ShelfMappings.ToShelfDto(story))
                .AsNoTracking()
                .ToListAsync();
            
            return shelves.Count > 0 ? Ok(shelves) : NoContent();
        }

        /// <summary>
        /// Create a new shelf
        /// </summary>
        /// <param name="data">Shelf data</param>
        /// <returns></returns>
        // POST: api/Shelves
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<ActionResult<ShelfDto>> PostShelfAsync(PostData data)
        {
            var uid = User?.GetNumericId();
            if (uid is null) return Unauthorized();
            
            var shelf = new Shelf
            {
                Name         = data.Name,
                Description  = data.Description,
                OwnerId      = (long) uid,
                IsPublic     = data.IsPublic,
                IsQuickAdd   = data.IsQuick,
                TrackUpdates = data.TrackUpdates,
                Color        = data.Color,
                IconId       = data.Icon
            };
            _context.Shelves.Add(shelf);
            await _context.SaveChangesAsync();
            return Ok();
        }

        /// <summary>
        /// Add the story with `storyId` to shelf of `shelfId`
        /// </summary>
        /// <param name="shelfId">ID of the shelf to add to</param>
        /// <param name="storyId">ID of the story to add</param>
        /// <returns></returns>
        // POST: api/Shelves/add/5/6
        [HttpPost("add/{shelfId:long}/{storyId:long}")]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<ActionResult> AddToShelfAsync(long shelfId, long storyId)
        {
            var uid = User?.GetNumericId();
            if (uid is null) return Unauthorized();
            
            var shelf = await _context.Shelves.FindAsync(shelfId);

            var storyExists = await _context.Stories.AnyAsync(s => s.Id == storyId);
            
            // Check existence
            if (shelf is null || !storyExists) return NotFound();
            // Check ownership
            if (shelf.OwnerId != uid) return Unauthorized();
            
            var exists = _context.ShelfStories.Any(ss => ss.ShelfId == shelfId && ss.StoryId == storyId);
            var shelfStory = new ShelfStory
            {
                ShelfId = shelfId,
                StoryId = storyId
            };

            if (exists)
                _context.ShelfStories.Remove(shelfStory);
            else
                _context.ShelfStories.Add(shelfStory);

            await _context.SaveChangesAsync();
            return Ok(shelfStory);
        }

        /// <summary>
        /// Edit the shelf of `id` and replace its data with `data`
        /// </summary>
        /// <param name="id">ID of the shelf to edit</param>
        /// <param name="data">New data of the shelf</param>
        /// <returns></returns>
        // PUT: api/Shelves/5
        [HttpPut("{id:long}")]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<ActionResult<ShelfDto>> PutShelfAsync(long id, PostData data)
        {
            var uid = User?.GetNumericId();
            if (uid is null) return Unauthorized();
            
            var shelf = await _context.Shelves.FindAsync(id);

            // Check existence
            if (shelf is null) return NotFound();
            // Check ownership
            if (shelf.OwnerId != uid) return Unauthorized();
            
            shelf.Name         = data.Name;
            shelf.Description  = data.Description;
            shelf.Color        = data.Color;
            shelf.IsPublic     = data.IsPublic;
            shelf.TrackUpdates = data.TrackUpdates;
            shelf.IsQuickAdd   = data.IsQuick;
            shelf.IconId       = data.Icon;
            
            await _context.SaveChangesAsync();
            return Ok(ShelfMappings.ToShelfDto().Compile().Invoke(shelf));
        }

        /// <summary>
        /// Delete the shelf of `id`
        /// </summary>
        /// <param name="id">ID of the shelf to delete</param>
        /// <returns></returns>
        // DELETE: api/Shelves/5
        [HttpDelete("{id:long}")]
        [Authorize]
        public async Task<ActionResult> DeleteShelfAsync(long id)
        {
            var uid = User?.GetNumericId();
            if (uid is null) return Unauthorized();
            
            var shelf = await _context.Shelves.FindAsync(id);
            
            // Check existence
            if (shelf is null) return NotFound();
            // Check ownership
            if (shelf.OwnerId != uid) return Unauthorized();
            
            _context.Shelves.Remove(shelf);
            await _context.SaveChangesAsync();
            
            return Ok();
        }

        /// <summary>
        /// Get validation data
        /// </summary>
        /// <returns></returns>
        // GET: api/Shelves/validation
        [HttpGet("validation")]
        public ActionResult GetShelfValidation() 
            => Ok(new {
                CTConfig.CShelf.MinNameLength,
                CTConfig.CShelf.MaxNameLength,
                CTConfig.CShelf.MaxDescriptionLength
            });

        public class PostData
        {
            [Required]
            [MinLength(CTConfig.CShelf.MinNameLength)]
            [MaxLength(CTConfig.CShelf.MaxNameLength)]
            public string Name { get; set; }
            
            [MaxLength(CTConfig.CShelf.MaxDescriptionLength)]
            public string Description { get; set; }

            public bool IsPublic { get; set; } = false;
            public bool IsQuick { get; set; } = false;
            public bool TrackUpdates { get; set; } = false;
            
            [MinLength(7)]
            [MaxLength(7)]
            public string Color { get; set; }
            public long Icon { get; set; }
        }

    }
}