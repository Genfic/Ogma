using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Castle.Core.Internal;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Ogma3.Data;
using Ogma3.Data.DTOs;
using Ogma3.Data.Models;

namespace Ogma3.Api
{
    [Route("api/[controller]", Name = nameof(ShelvesController))]
    [ApiController]
    public class ShelvesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<User> _userManager;
        
        public ShelvesController(ApplicationDbContext context, UserManager<User> userManager)
        {
            _context = context;
            _userManager = userManager;
        }
        

        /// <summary>
        /// Get all shelves that belong to user of `name`
        /// </summary>
        /// <param name="name">Name of the shelf owner, of null for currently logged-in user</param>
        /// <param name="story">Optional story ID parameter to check whether the story is on the shelf</param>
        /// <returns>List of `ShelfFromApiDTO` objects</returns>
        // GET: api/Shelves/user?name=JohnSmith&story=5
        [HttpGet("user")]
        public async Task<ActionResult<IEnumerable<ShelfFromApiDTO>>> GetUserShelvesAsync([FromQuery]string? name, [FromQuery]int? story)
        {
            var user = name.IsNullOrEmpty()
                ? await _userManager.GetUserAsync(User) 
                : await _context.Users.FirstOrDefaultAsync(u => u.NormalizedUserName == name.ToUpper());

            if (user == null) return NotFound();
            
            var shelves = await _context.Shelves
                .Where(s => s.Owner == user && (name == null || s.IsPublic))
                .Include(s => s.ShelfStories)
                .Include(s => s.Icon)
                .ToListAsync();
            
            return Ok(shelves.Select(s => ShelfFromApiDTO.FromShelf(s, story)));
        }

        /// <summary>
        /// Get shelf by ID
        /// </summary>
        /// <param name="id">ID of the shelf</param>
        /// <returns></returns>
        // GET: api/Shelves/5
        [HttpGet("{id}")]
        public async Task<ActionResult<ShelfFromApiDTO>> GetShelfAsync(int id)
        {
            var shelf = await _context.Shelves
                .Where(s => s.Id == id)
                .Include(s => s.ShelfStories)
                    .ThenInclude(ss => ss.Story)
                        .ThenInclude(s => s.StoryTags)
                            .ThenInclude(st => st.Tag)
                                .ThenInclude(t => t.Namespace)
                .Include(s => s.ShelfStories)
                    .ThenInclude(ss => ss.Story)
                        .ThenInclude(s => s.Author)
                .Include(s => s.ShelfStories)
                    .ThenInclude(ss => ss.Story)
                        .ThenInclude(s => s.Rating)
                .Include(s => s.Icon)
                .FirstOrDefaultAsync();

                if (shelf == null) return NotFound();
            
            return Ok(ShelfFromApiDTO.FromShelf(shelf));
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
        public async Task<ActionResult<ShelfFromApiDTO>> PostShelfAsync(PostData data)
        {
            var user = await _userManager.GetUserAsync(User);
            var shelf = new Shelf
            {
                Name        = data.Name,
                Description = data.Description,
                Owner       = user,
                IsPublic    = data.IsPublic,
                IsQuickAdd  = data.IsQuick,
                Color       = data.Color,
                IconId      = data.Icon
            };
            _context.Shelves.Add(shelf);
            await _context.SaveChangesAsync();
            return Ok(ShelfFromApiDTO.FromShelf(shelf));
        }

        /// <summary>
        /// Add the story with `storyId` to shelf of `shelfId`
        /// </summary>
        /// <param name="shelfId">ID of the shelf to add to</param>
        /// <param name="storyId">ID of the story to add</param>
        /// <returns></returns>
        // POST: api/Shelves/add/5/6
        [HttpPost("add/{shelfId}/{storyId}")]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<ActionResult> AddToShelfAsync(int shelfId, int storyId)
        {
            var shelf = await _context.Shelves.FindAsync(shelfId);
            var story = await _context.Stories.FindAsync(storyId);
            var user  = await _userManager.GetUserAsync(User);
            
            // Check existence
            if (shelf == null || story == null) return NotFound();
            // Check ownership
            if (shelf.Owner != user) return Forbid();
            
            var exists = _context.ShelfStories.Any(ss => ss.Shelf == shelf && ss.Story == story);

            var shelfStory = new ShelfStory
            {
                Shelf = shelf,
                Story = story
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
        [HttpPut("{id}")]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<ActionResult<ShelfFromApiDTO>> PutShelfAsync(int id, PostData data)
        {
            var shelf = await _context.Shelves.FindAsync(id);
            var user  = await _userManager.GetUserAsync(User);

            // Check existence
            if (shelf == null) return NotFound();
            // Check ownership
            if (shelf.Owner != user) return Forbid();
            
            shelf.Name        = data.Name;
            shelf.Description = data.Description;
            shelf.Color       = data.Color;
            shelf.IsPublic    = data.IsPublic;
            shelf.IsQuickAdd  = data.IsQuick;
            shelf.IconId      = data.Icon;
            
            await _context.SaveChangesAsync();
            return Ok(ShelfFromApiDTO.FromShelf(shelf));
        }

        /// <summary>
        /// Delete the shelf of `id`
        /// </summary>
        /// <param name="id">ID of the shelf to delete</param>
        /// <returns></returns>
        // DELETE: api/Shelves/5
        [HttpDelete("{id}")]
        [Authorize]
        public async Task<ActionResult> DeleteShelfAsync(int id)
        {
            var user = await _userManager.GetUserAsync(User);
            var shelf = await _context.Shelves.FindAsync(id);
            
            // Check existence
            if (shelf == null) return NotFound();
            // Check ownership
            if (shelf.Owner != user) return Forbid();
            
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
        {
            return Ok(new
            {
                CTConfig.Shelf.MinNameLength,
                CTConfig.Shelf.MaxNameLength,
                CTConfig.Shelf.MaxDescriptionLength
            });
        }

        public class PostData
        {
            [Required]
            [MinLength(CTConfig.Shelf.MinNameLength)]
            [MaxLength(CTConfig.Shelf.MaxNameLength)]
            public string Name { get; set; }
            
            [MaxLength(CTConfig.Shelf.MaxDescriptionLength)]
            public string Description { get; set; }

            public bool IsPublic { get; set; } = false;
            public bool IsQuick { get; set; } = false;
            
            [MinLength(7)]
            [MaxLength(7)]
            public string Color { get; set; }

            public int Icon { get; set; }
        }

    }
}