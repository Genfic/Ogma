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
        public InputModel Input { get; set; }
        
        public class InputModel
        {
            public long Id { get; set; }
            
            [Required]
            [StringLength(CTConfig.CBlogpost.MaxTitleLength,
                ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.",
                MinimumLength = CTConfig.CBlogpost.MinTitleLength)]
            public string Title { get; set; }
            
            [Required]
            [StringLength(CTConfig.CBlogpost.MaxBodyLength,
                ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.",
                MinimumLength = CTConfig.CBlogpost.MinBodyLength)]
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
            
            Input = new InputModel
            {
                Id = post.Id,
                Title = post.Title,
                Body = post.Body
            };
            
            return Page();
        }

        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync(int? id)
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }
            // Get logged in user
            var user = await _userManager.GetUserAsync(User);
            // Get post and make sure the user matches
            var post = await _context.Blogposts
                .FirstOrDefaultAsync(m => m.Id == id && m.Author == user);
            // 404 if no post found
            if (post == null) return NotFound();

            post.Title = Input.Title.Trim();
            post.Slug = Input.Title.Trim().Friendlify();
            post.Body = Input.Body.Trim();
            
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!BlogpostExists(Input.Id))
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
