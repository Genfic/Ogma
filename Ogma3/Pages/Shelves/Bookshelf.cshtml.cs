using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Ogma3.Data.Repositories;
using Ogma3.Pages.Shared;

namespace Ogma3.Pages.Shelves
{
    public class Bookshelf : PageModel
    {
        private readonly BookshelfRepository _bookshelfRepo;

        public Bookshelf(BookshelfRepository bookshelfRepo)
        {
            _bookshelfRepo = bookshelfRepo;
        }

        public BookshelfDetails Shelf { get; set; }
        
        public async Task<IActionResult> OnGetAsync(int id, string? slug)
        {
            Shelf = await _bookshelfRepo.GetBookshelfDetails(id);
            
            if (Shelf == null || !Shelf.IsPublic) return NotFound();

            return Page();
        }
    }
}