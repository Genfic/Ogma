using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Ogma3.Data;
using Ogma3.Data.Models;

namespace Ogma3.Pages
{
    public class Faq : PageModel
    {
        private readonly ApplicationDbContext _context;

        public Faq(ApplicationDbContext context)
        {
            _context = context;
        }

        public List<Data.Models.Faq> Faqs { get; set; }
        
        public async Task OnGetAsync()
        {
            Faqs = await _context.Faqs
                .AsNoTracking()
                .ToListAsync();
        }
    }
}