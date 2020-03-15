using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Ogma3.Data;
using Ogma3.Data.DTOs;
using Ogma3.Data.Models;
using Utils;

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
        

        // GET: api/Shelves/user/JohnSmith
        [HttpGet("user/{name?}")]
        public async Task<ActionResult<IEnumerable<ShelfFromApiDTO>>> GetUserShelvesAsync(string name)
        {
            var user = name == null 
                ? await _userManager.GetUserAsync(User) 
                : await _context.Users.FirstOrDefaultAsync(u => u.NormalizedUserName == name.ToUpper());

            if (user == null) return NotFound();
            
            var shelves = await _context.Shelves
                .Where(s => s.Owner == user)
                .Include(s => s.ShelfStories)
                .Include(s => s.Icon)
                .ToListAsync();
            return Ok(shelves.Select(ShelfFromApiDTO.FromShelf));
        }

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

        [HttpPost("add/{shelfId}/{storyId}")]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<ActionResult> AddShelfAsync(int shelfId, int storyId)
        {
            var shelf = await _context.Shelves.FindAsync(shelfId);
            var story = await _context.Stories.FindAsync(storyId);

            if (shelf == null || story == null) return NotFound();

            _context.ShelfStories.Add(new ShelfStory
            {
                Shelf = shelf,
                ShelfId = shelf.Id,
                Story = story,
                StoryId = story.Id
            });

            await _context.SaveChangesAsync();
            return Ok();
        }

        // PUT: api/Shelves/5
        [HttpPut("{id}")]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<ActionResult<ShelfFromApiDTO>> PutShelfAsync(int id, PostData data)
        {
            var shelf = await _context.Shelves.FindAsync(id);

            if (shelf == null) return NotFound();
            
            shelf.Name        = data.Name;
            shelf.Description = data.Description;
            shelf.Color       = data.Color;
            shelf.IsPublic    = data.IsPublic;
            shelf.IsQuickAdd  = data.IsQuick;
            shelf.IconId      = data.Icon;
            
            await _context.SaveChangesAsync();
            return Ok(ShelfFromApiDTO.FromShelf(shelf));
        }

        [HttpDelete("{id}")]
        [Authorize]
        public async Task<ActionResult> DeleteShelfAsync(int id)
        {
            var user = await _userManager.GetUserAsync(User);
            var shelf = await _context.Shelves.FindAsync(id);
            
            if (shelf == null) return NotFound();
            if (shelf.Owner != user) return Forbid();
            
            _context.Shelves.Remove(shelf);
            await _context.SaveChangesAsync();
            
            return Ok();
        }

        // GET: api/Shelves/validation
        [HttpGet("validation")]
        public ActionResult GetShelfValidation()
        {
            return Ok(new
            {
                CTConfig.Shelf.MinNameLength,
                CTConfig.Shelf.MaxNameLength,
                CTConfig.Shelf.MaxDescriptionLength,
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