using System;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Ogma3.Data;
using Ogma3.Data.Chapters;
using Ogma3.Infrastructure.Extensions;
using Ogma3.Pages.Shared;

namespace Ogma3.Pages
{
    public class ChapterModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;

        public ChapterModel(ApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public ChapterDetails Chapter { get; set; }

        public class ChapterDetails
        {
            public long StoryId { get; init; }
            public string StoryTitle { get; init; }
            public string StorySlug { get; init; }
            public long StoryAuthorId { get; init; }
            public string StoryRatingName { get; init; }
            public long Id { get; init; }
            public string Title { get; init; }
            public string Slug { get; init; }
            public uint Order { get; init; }
            public DateTime? PublicationDate { get; init; }
            public string Body { get; init; }
            public string StartNotes { get; init; }
            public string EndNotes { get; init; }
            public CommentsThreadDto CommentsThread { get; init; }
            public long? ContentBlockId { get; init; }
            public ChapterMicroDto? Previous { get; set; }
            public ChapterMicroDto? Next { get; set; }
        }

        public class ChapterMicroDto
        {
            public long Id { get; init; }
            public string Title { get; init; }
            public string Slug { get; init; }

            public uint Order { get; set; }
        }

        public class MappingProfile : Profile
        {
            public MappingProfile()
            {
                CreateMap<Chapter, ChapterDetails>();
                CreateMap<Chapter, ChapterMicroDto>();
            }
        }

        public async Task<IActionResult> OnGetAsync(long id, string? slug)
        {
            var uid = User.GetNumericId();

            Chapter = await _context.Chapters
                .Where(c => c.Id == id)
                .Where(c => c.PublicationDate != null || c.Story.AuthorId == uid)
                .Where(c => c.ContentBlockId == null || c.Story.AuthorId == uid)
                .ProjectTo<ChapterDetails>(_mapper.ConfigurationProvider)
                .FirstOrDefaultAsync();
            
            if (Chapter is null) return NotFound();
            
            Chapter.Previous = await _context.Chapters
                .Where(c => c.StoryId == Chapter.StoryId)
                .Where(c => c.PublicationDate != null)
                .Where(c => c.Order < Chapter.Order)
                .OrderBy(c => c.Order)
                .ProjectTo<ChapterMicroDto>(_mapper.ConfigurationProvider)
                .LastOrDefaultAsync();
            Chapter.Next = await _context.Chapters
                .Where(c => c.StoryId == Chapter.StoryId)
                .Where(c => c.PublicationDate != null)
                .Where(c => c.Order > Chapter.Order)
                .OrderBy(c => c.Order)
                .ProjectTo<ChapterMicroDto>(_mapper.ConfigurationProvider)
                .FirstOrDefaultAsync();
            
            Chapter.CommentsThread.Type = nameof(Data.Chapters.Chapter);
            
            return Page();
        }
    }
}