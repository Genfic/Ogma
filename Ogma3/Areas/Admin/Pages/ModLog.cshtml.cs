using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Humanizer;
using Markdig.Extensions.Footnotes;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Ogma3.Data;
using Ogma3.Data.Models;

namespace Ogma3.Areas.Admin.Pages
{
    public class ModLog : PageModel
    {
        private readonly ApplicationDbContext _context;

        public ModLog(ApplicationDbContext context)
        {
            _context = context;
        }
        
        public ICollection<ModeratorAction> Actions { get; set; }

        public async Task OnGet()
        {
            Actions = await _context.ModeratorActions
                .OrderByDescending(ma => ma.DateTime)
                .ToListAsync();
        }
    }
}