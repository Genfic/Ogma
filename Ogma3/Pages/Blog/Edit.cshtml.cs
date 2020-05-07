using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Ogma3.Data;
using Utils;

namespace Ogma3.Pages.Blog
{
    [Authorize]
    public class EditModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private OgmaUserManager _userManager;

        public EditModel(ApplicationDbContext context, OgmaUserManager userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        [BindProperty]
        public InputModel Blogpost { get; set; }
        
        public class InputModel
        {
            public long Id { get; set; }
            
            [Required]
            [StringLength(CTConfig.Blogpost.MaxTitleLength,
                ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.",
                MinimumLength = CTConfig.Blogpost.MinTitleLength)]
            public string Title { get; set; }
            
            [Required]
            [StringLength(CTConfig.Blogpost.MaxBodyLength,
                ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.",
                MinimumLength = CTConfig.Blogpost.MinBodyLength)]
            public string Body { get; set; }
        }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            // Get logged in user
            var user = await _userManager.GetUserAsync(User);
            // Get post and make sure the user matches
            var post = await _context.Blogposts
                .FirstOrDefaultAsync(m => m.Id == id && m.Author == user);

            if (post == null) return NotFound();
            
            Blogpost = new InputModel
            {
                Id = post.Id,
                Title = post.Title,
                Body = post.Body
            };
            
            return Page();
        }

        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }
            // Get logged in user
            var user = await _userManager.GetUserAsync(User);
            // Get post and make sure the user matches
            var post = await _context.Blogposts
                .FirstOrDefaultAsync(m => m.Id == Blogpost.Id && m.Author == user);
            // 404 if no post found
            if (post == null) return NotFound();

            post.Title = Blogpost.Title.Trim();
            post.Slug = Blogpost.Title.Trim().Friendlify();
            post.Body = Blogpost.Body.Trim();
            
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!BlogpostExists(Blogpost.Id))
                {
                    return NotFound();
                }
                throw;
            }

            return RedirectToPage("./Post", new { id = post.Id, slug = post.Slug });
        }

        private bool BlogpostExists(long id)
        {
            return _context.Blogposts.Any(e => e.Id == id);
        }
    }
}
