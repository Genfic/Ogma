using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Ogma3.Data;
using Ogma3.Pages.Shared;
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
            public string Tags { get; set; }
            
            public ChapterMinimal? ChapterMinimal { get; set; }
            public StoryMinimal? StoryMinimal { get; set; }
        }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            // Get logged in user
            var uid = User.GetNumericId();
            if (uid == null) return Unauthorized();
            
            // Get post and make sure the user matches
            var post = await _context.Blogposts
                .AsNoTracking()
                .FirstOrDefaultAsync(m => m.Id == id && m.AuthorId == uid);

            if (post == null) return NotFound();
            
            Input = new InputModel
            {
                Id = post.Id,
                Title = post.Title,
                Body = post.Body,
                Tags = post.Hashtags
                    .Select(t => t.Trim('#'))
                    .ToArray()
                    .JoinToString(", ")
            };

            if (post.AttachedStoryId.HasValue)
            {
                Input.StoryMinimal = await _context.Stories
                    .Where(s => s.Id == post.AttachedStoryId)
                    .ProjectTo<StoryMinimal>(_mapper.ConfigurationProvider)
                    .AsNoTracking()
                    .FirstOrDefaultAsync();
            } 
            else if (post.AttachedChapterId.HasValue)
            {
                Input.ChapterMinimal = await _context.Chapters
                    .Where(c => c.Id == post.AttachedChapterId)
                    .ProjectTo<ChapterMinimal>(_mapper.ConfigurationProvider)
                    .AsNoTracking()
                    .FirstOrDefaultAsync();
            }
            
            return Page();
        }

        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync(long id)
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }
            
            // Get logged in user
            var uid = User.GetNumericId();
            if (uid == null) return Unauthorized();
            
            // Get post and make sure the user matches
            var post = await _context.Blogposts
                .FirstOrDefaultAsync(m => m.Id == id && m.AuthorId == uid);
            
            // 404 if no post found
            if (post == null) return NotFound();
            
            // Create array of hashtags
            var tags = Input.Tags
                .Split(',')
                .Where(t => !string.IsNullOrWhiteSpace(t))
                .ToList()
                .Select(t => '#' + t.Trim(' ', '#', ',').Friendlify())
                .Distinct()
                .ToArray();

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
