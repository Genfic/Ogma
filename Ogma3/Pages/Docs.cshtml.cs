using System.Collections.Generic;
using System.Linq;
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
        public List<Document> Versions { get; set; }
        
        public async Task<IActionResult> OnGetAsync(long id, string? slug)
        {
            Document = await _context.Documents
                .AsNoTracking()
                .Where(d => d.Id == id)
                .FirstOrDefaultAsync();
            
            if (Document == null)
                return NotFound();
            
            Versions = await _context.Documents
                .AsNoTracking()
                .Where(d => d.GroupId == Document.GroupId)
                .ToListAsync();
            
            return Page();
        }
    }
}