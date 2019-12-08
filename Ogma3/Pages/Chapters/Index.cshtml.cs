using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Ogma3.Data;
using Ogma3.Data.Models;

namespace Ogma3.Pages.Chapters
{
    public class IndexModel : PageModel
    {
        private readonly Ogma3.Data.ApplicationDbContext _context;

        public IndexModel(Ogma3.Data.ApplicationDbContext context)
        {
            _context = context;
        }

        public IList<Chapter> Chapter { get;set; }

        public async Task OnGetAsync()
        {
            Chapter = await _context.Chapters.ToListAsync();
        }
    }
}
