using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Ogma3.Data;
using Ogma3.Data.Models;

namespace Ogma3.Pages
{
    public class Docs : PageModel
    {
        private readonly ApplicationDbContext _context;

        public Docs(ApplicationDbContext context)
        {
            _context = context;
        }

        public Document Document { get; set; }
        
        public async Task<IActionResult> OnGetAsync(string? doc)
        {
            Document = await _context.Documents
                .AsNoTracking()
                .FirstOrDefaultAsync(d => d.Slug == doc);

            if (Document == null)
                return NotFound();
            
            return Page();
        }
    }
}