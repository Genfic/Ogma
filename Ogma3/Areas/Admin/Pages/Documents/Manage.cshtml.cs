using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Ogma3.Data;
using Ogma3.Data.Models;

namespace Ogma3.Areas.Admin.Pages.Documents
{
    public class Manage : PageModel
    {
        private readonly ApplicationDbContext _context;

        public Manage(ApplicationDbContext context)
        {
            _context = context;
        }
        public List<Document> Documents { get; set; }
        
        public async Task<IActionResult> OnGetAsync()
        {
            Documents = await _context.Documents.ToListAsync();

            if (Documents.Count <= 0)
                return NotFound();
            
            return Page();
        }
    }
}