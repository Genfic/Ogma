using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Ogma3.Data;
using Ogma3.Data.DTOs;
using Ogma3.Data.Models;

namespace Ogma3.Pages
{
    public class TagModel : PageModel
    {
        private readonly ILogger<TagModel> _logger;
        private readonly ApplicationDbContext _context;
        
        public TagModel(ApplicationDbContext context, ILogger<TagModel> logger)
        {
            _context = context;
            _logger = logger;
        }

        public TagDTO Tag { get; set; }
        public IEnumerable<Story> Stories { get; set; }

        public async Task<IActionResult> OnGetAsync(long id, string? slug)
        {
            var tag = await _context.Tags
                .Where(t => t.Id == id)
                .Include(t => t.Namespace)
                .AsNoTracking()
                .FirstOrDefaultAsync();
            Tag = TagDTO.FromTag(tag);
            
            if (Tag == null) return NotFound();

            Stories = await _context.Stories
                .Include(s => s.Author)
                .Include(s => s.Rating)
                .Include(s => s.StoryTags)
                .ThenInclude(st => st.Tag)
                .ThenInclude(t => t.Namespace)
                .Where(s => s.StoryTags.Any(st => st.TagId == id))
                .AsNoTracking()
                .ToListAsync();

            return Page();
        }
    }
}
