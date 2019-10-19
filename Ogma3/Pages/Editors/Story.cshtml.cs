using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Ogma3.Data;
using Ogma3.Data.Models;

namespace Ogma3.Pages.Editors
{
    [Authorize]
    public class StoryModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public StoryModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public List<Rating> Ratings { get; set; }
        
        public async Task OnGetAsync()
        {
            Ratings = await _context.Ratings.ToListAsync();
        }
    }
}