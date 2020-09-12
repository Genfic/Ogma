using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Ogma3.Data;
using Ogma3.Data.Models;

namespace Ogma3.Pages.Club.Forums
{
    public class DeleteModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public DeleteModel(ApplicationDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public ClubThread ClubThread { get; set; }

        public async Task<IActionResult> OnGetAsync(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            ClubThread = await _context.ClubThreads.FirstOrDefaultAsync(m => m.Id == id);

            if (ClubThread == null)
            {
                return NotFound();
            }
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            ClubThread = await _context.ClubThreads.FindAsync(id);

            if (ClubThread != null)
            {
                _context.ClubThreads.Remove(ClubThread);
                await _context.SaveChangesAsync();
            }

            return RedirectToPage("./Index");
        }
    }
}
