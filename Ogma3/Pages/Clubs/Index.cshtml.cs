using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Ogma3.Data;
using Ogma3.Data.Models;
using Ogma3.Pages.Shared;

namespace Ogma3.Pages.Clubs
{
    public class IndexModel : PageModel
    {
        private readonly Ogma3.Data.ApplicationDbContext _context;

        public IndexModel(Ogma3.Data.ApplicationDbContext context)
        {
            _context = context;
        }

        public IList<Club> Club { get;set; }

        public const int PerPage = 2;
        public PaginationModel PaginationModel { get; set; }
        public async Task OnGetAsync()
        {
            Club = await _context.Clubs.ToListAsync();
        }
    }
}
