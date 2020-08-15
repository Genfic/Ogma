using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Ogma3.Data;
using Utils.Extensions;

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
                ErrorMessage = CTConfig.CBlogpost.ValidateLengthMsg,
                MinimumLength = CTConfig.CBlogpost.MinTitleLength)]
            public string Title { get; set; }
            
            [Required]
            [StringLength(CTConfig.CBlogpost.MaxBodyLength,
                ErrorMessage = CTConfig.CBlogpost.ValidateLengthMsg,
                MinimumLength = CTConfig.CBlogpost.MinBodyLength)]
            public string Body { get; set; }
        }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            // Get logged in user
            var user = await _userManager.GetUserAsync(User);
            // Get post and make sure the user matches
            var post = await _context.Blogposts
                .AsNoTracking()
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
            
            // Find hashtags
            var rgx = new Regex(@"\#[\w\-]+");
            var tags = rgx
                .Matches(Input.Body)
                .Select(m => m.Value)
                .Distinct()
                .Take(CTConfig.CBlogpost.MaxTagsAmount)
                .ToList();

            post.Title = Input.Title.Trim();
            post.Slug = Input.Title.Trim().Friendlify();
            post.Body = Input.Body.Trim();
            post.WordCount = Input.Body.Trim().Split(' ', '\t', '\n').Length;
            post.Hashtags = tags.ToArray();
            
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
