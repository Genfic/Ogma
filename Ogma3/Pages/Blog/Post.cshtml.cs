using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Ogma3.Data;
using Ogma3.Data.DTOs;
using Ogma3.Data.Models;
using Ogma3.Data.Repositories;

namespace Ogma3.Pages.Blog
{
    public class DetailsModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private ProfileBarRepository _profileBarRepo;

        public DetailsModel(ApplicationDbContext context, ProfileBarRepository profileBarRepo)
        {
            _context = context;
            _profileBarRepo = profileBarRepo;
        }

        public Blogpost Blogpost { get; set; }
        public ProfileBarDTO ProfileBar { get; set; }

        public async Task<IActionResult> OnGetAsync(long id, string? slug)
        {
            Blogpost = await _context.Blogposts
                .Where(b => b.Id == id)
                .Include(b => b.Author)
                .Include(b => b.CommentsThread)
                .AsNoTracking()
                .FirstOrDefaultAsync();

            if (Blogpost == null)
            {
                return NotFound();
            }
            
            ProfileBar = await _profileBarRepo.GetAsync(Blogpost.Author.NormalizedUserName);
            
            return Page();
        }

    }
}
