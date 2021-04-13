using System;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using FluentValidation;
using LinqToDB;
using LinqToDB.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Ogma3.Data;
using Ogma3.Data.Blogposts;
using Ogma3.Infrastructure.CustomValidators;
using Ogma3.Infrastructure.Extensions;
using Ogma3.Pages.Shared.Minimals;
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
        public PostData Input { get; set; }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            // Get logged in user
            var uid = User.GetNumericId();
            if (uid == null) return Unauthorized();
            
            // Get post and make sure the user matches
            Input = await _context.Blogposts
                .Where(m => m.Id == id && m.AuthorId == uid)
                .ProjectTo<PostData>(_mapper.ConfigurationProvider)
                .FirstOrDefaultAsyncEF();

            if (Input == null) return NotFound();
            
            return Page();
        }

        public class PostData
        {
            public long Id { get; set; }
            public string Title { get; set; }
            public string Body { get; set; }
            public string Tags { get; set; }
            public ChapterMinimal? AttachedChapter { get; set; }
            public StoryMinimal? AttachedStory { get; set; }
            public bool IsUnavailable { get; set; }
            public bool Published { get; set; }
        }
        
        public class PostDataValidation : AbstractValidator<PostData>
        {
            public PostDataValidation()
            {
                RuleFor(b => b.Title)
                    .NotEmpty()
                    .Length(CTConfig.CBlogpost.MinTitleLength, CTConfig.CBlogpost.MaxTitleLength);
                RuleFor(b => b.Body)
                    .NotEmpty()
                    .Length(CTConfig.CBlogpost.MinBodyLength, CTConfig.CBlogpost.MaxBodyLength);
                RuleFor(b => b.Tags)
                    .HashtagsFewerThan(CTConfig.CBlogpost.MaxTagsAmount)
                    .HashtagsShorterThan(CTConfig.CBlogpost.MaxTagLength);
            }
        }
        public class MappingProfile : Profile
        {
            public MappingProfile() => CreateMap<Blogpost, PostData>()
                .ForMember(pd => pd.Tags, opts 
                        => opts.MapFrom(b => string.Join(", ", b.Hashtags)));
        }

        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync(long id)
        {
            if (!ModelState.IsValid) return Page();
            
            // Get logged in user
            var uid = User.GetNumericId();
            if (uid == null) return Unauthorized();
            
            await _context.Blogposts
                .Where(m => m.Id == id && m.AuthorId == uid)
                .Set(b => b.Title, Input.Title.Trim())
                .Set(b => b.Slug, Input.Title.Trim().Friendlify())
                .Set(b => b.Body, Input.Body.Trim())
                .Set(b => b.WordCount, Input.Body.Words())
                .Set(b => b.Hashtags, Input.Tags?.ParseHashtags() ?? Array.Empty<string>())
                .Set(b => b.IsPublished, Input.Published)
                .UpdateAsync();

            return RedirectToPage("./Post", new { id, slug = Input.Title.Trim().Friendlify() });
        }
    }
}
