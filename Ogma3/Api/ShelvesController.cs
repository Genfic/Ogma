using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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

        [HttpGet("user/{name?}")]
        public async Task<ActionResult<IEnumerable<ShelfFromApiDTO>>> GetUserShelvesAsync(string name)
        {
            var user = name == null 
                ? await _userManager.GetUserAsync(User) 
                : await _context.Users.FirstOrDefaultAsync(u => u.NormalizedUserName == name.ToUpper());

            if (user == null) return NotFound();
            
            var shelves = await _context.Shelves.Where(s => s.Owner == user).ToListAsync();
            return Ok(shelves.Select(ShelfFromApiDTO.FromShelf));
        }

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
                .FirstOrDefaultAsync();

                if (shelf == null) return NotFound();
            
            return Ok(ShelfFromApiDTO.FromShelf(shelf));
        }

        [HttpPost]
        [Authorize]
        public async Task<ActionResult> PostShelfAsync(PostData data)
        {
            var user = await _userManager.GetUserAsync(User);
            var shelf = new Shelf
            {
                Name = data.Name,
                Owner = user,
                IsDefault = false,
                IsPublic = data.IsPublic
            };
            _context.Shelves.Add(shelf);
            await _context.SaveChangesAsync();
            return Ok(ShelfFromApiDTO.FromShelf(shelf));
        }

        public class PostData
        {
            public string Name { get; set; }
            public bool IsPublic { get; set; }
        }
    }
}