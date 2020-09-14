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
using Ogma3.Pages.Shared;

namespace Ogma3.Pages.Blog
{
    public class DetailsModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private UserRepository _userRepo;

        public DetailsModel(ApplicationDbContext context, UserRepository userRepo)
        {
            _context = context;
            _userRepo = userRepo;
        }

        public Blogpost Blogpost { get; set; }
        public ProfileBar ProfileBar { get; set; }

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
            
            ProfileBar = await _userRepo.GetProfileBar(Blogpost.Author.NormalizedUserName);
            
            return Page();
        }

    }
}
