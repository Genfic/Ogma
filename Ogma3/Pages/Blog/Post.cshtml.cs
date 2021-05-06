using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Ogma3.Data;
using Ogma3.Data.Blogposts;
using Ogma3.Infrastructure.Extensions;
using Ogma3.Pages.Shared.Minimals;

namespace Ogma3.Pages.Blog
{
    public class DetailsModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;

        public DetailsModel(ApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public Details Blogpost { get; private set; }
        
        public class Details
        {
            public long Id { get; init; }
            public long AuthorId { get; init; }
            public string AuthorUserName { get; init; }
            public string Title { get; init; }
            public string Slug { get; init; }
            public DateTime PublishDate { get; init; }
            public string Body { get; init; }
            public IEnumerable<string> Hashtags { get; init; }
            public long CommentsThreadId { get; init; }
            public bool IsPublished { get; init; }
            public int CommentsCount { get; init; }
            
            public ChapterMinimal AttachedChapter { get; set; }
            public StoryMinimal AttachedStory { get; set; }
            public bool IsUnavailable { get; set; }
            public long? ContentBlockId { get; set; }
        }
        
        public class MappingProfile : Profile
        {
            public MappingProfile() => CreateMap<Blogpost, Details>();
        }

        public async Task<IActionResult> OnGetAsync(long id, string? slug)
        {
            Blogpost = await _context.Blogposts
                .Where(b => b.Id == id)
                .ProjectTo<Details>(_mapper.ConfigurationProvider)
                .FirstOrDefaultAsync();

            if (Blogpost == null) return NotFound();
            if (!Blogpost.IsPublished && User.GetNumericId() != Blogpost.AuthorId) return NotFound();

            if (Blogpost.AttachedChapter is not null && !Blogpost.AttachedChapter.IsPublished)
            {
                Blogpost.IsUnavailable = true;
            }
            else if (Blogpost.AttachedStory is not null && !Blogpost.AttachedStory.IsPublished)
            {
                Blogpost.IsUnavailable = true;
            }
            
            return Page();
        }

    }
}
