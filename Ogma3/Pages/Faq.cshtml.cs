using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Ogma3.Data;

namespace Ogma3.Pages
{
    public class Faq : PageModel
    {
        private readonly ApplicationDbContext _context;

        public Faq(ApplicationDbContext context)
        {
            _context = context;
        }

        public List<Data.Faqs.Faq> Faqs { get; set; }
        
        public async Task OnGetAsync()
        {
            Faqs = await _context.Faqs
                .AsNoTracking()
                .ToListAsync();
        }
    }
}