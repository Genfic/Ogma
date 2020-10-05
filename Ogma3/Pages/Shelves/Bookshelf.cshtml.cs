using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Ogma3.Data;
using Ogma3.Pages.Shared;
using Utils.Extensions;

namespace Ogma3.Pages.Shelves
{
    public class Bookshelf : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;

        public Bookshelf(ApplicationDbContext context, IMapper mapper)
        {
            _mapper = mapper;
            _context = context;
        }

        public BookshelfDetails Shelf { get; set; }
        public async Task<IActionResult> OnGetAsync(int id, string? slug)
        {
            Shelf = await _context.Shelves
                .Where(s => s.Id == id)
                .ProjectTo<BookshelfDetails>(_mapper.ConfigurationProvider, new { currentUser = User.GetNumericId() })
                .AsNoTracking()
                .FirstOrDefaultAsync();
            
            if (Shelf == null || !Shelf.IsPublic) return NotFound();

            return Page();
        }
    }
}