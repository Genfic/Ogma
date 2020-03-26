using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Ogma3.Data;
using Utils;

namespace Ogma3.Pages.Blog
{
    public class EditModel : PageModel
    {
        private readonly Ogma3.Data.ApplicationDbContext _context;

        public EditModel(Ogma3.Data.ApplicationDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public InputModel Blogpost { get; set; }
        
        public class InputModel
        {
            public int Id { get; set; }
            
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
            var post = await _context.Blogposts.FirstOrDefaultAsync(m => m.Id == id);

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

            var post = await _context.Blogposts.FirstOrDefaultAsync(m => m.Id == Blogpost.Id);
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
                else
                {
                    throw;
                }
            }

            return RedirectToPage("./Post", new { id = post.Id, slug = post.Slug });
        }

        private bool BlogpostExists(int id)
        {
            return _context.Blogposts.Any(e => e.Id == id);
        }
    }
}
