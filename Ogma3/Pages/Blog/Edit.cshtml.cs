using System;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using LinqToDB;
using LinqToDB.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Ogma3.Data;
using Ogma3.Data.Blogposts;
using Ogma3.Infrastructure.Extensions;
using Utils.Extensions;

namespace Ogma3.Pages.Blog
{
    [Authorize]
    public class EditModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;

        public EditModel(ApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        [BindProperty]
        public BlogpostEditDto Input { get; set; }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            // Get logged in user
            var uid = User.GetNumericId();
            if (uid == null) return Unauthorized();
            
            // Get post and make sure the user matches
            Input = await _context.Blogposts
                .Where(m => m.Id == id && m.AuthorId == uid)
                .ProjectTo<BlogpostEditDto>(_mapper.ConfigurationProvider)
                .AsNoTracking()
                .FirstOrDefaultAsyncEF();

            if (Input == null) return NotFound();
            
            return Page();
        }

        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync(long id)
        {
            if (!ModelState.IsValid) return Page();
            
            // Get logged in user
            var uid = User.GetNumericId();
            if (uid == null) return Unauthorized();
            
            // Create array of hashtags
            var tags = Input.Tags?
                .Split(',')
                .Where(t => !string.IsNullOrWhiteSpace(t))
                .Select(t => t.Trim(' ', '#', ',').Friendlify())
                .Distinct()
                .ToArray() ?? Array.Empty<string>();

            await _context.Blogposts
                .Where(m => m.Id == id && m.AuthorId == uid)
                .Set(b => b.Title, Input.Title.Trim())
                .Set(b => b.Slug, Input.Title.Trim().Friendlify())
                .Set(b => b.Body, Input.Body.Trim())
                .Set(b => b.WordCount, Input.Body.Words())
                .Set(b => b.Hashtags, tags)
                .Set(b => b.IsPublished, Input.Published)
                .UpdateAsync();

            return RedirectToPage("./Post", new { id, slug = Input.Title.Trim().Friendlify() });
        }
    }
}
