using System.ComponentModel.DataAnnotations;
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
using Ogma3.Data.Chapters;
using Ogma3.Infrastructure.Extensions;
using Utils.Extensions;

namespace Ogma3.Pages.Chapters
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

        public async Task<IActionResult> OnGetAsync(long id)
        {
            // Get chapter
            var chapter = await _context.Chapters
                .Where(c => c.Id == id)
                .ProjectTo<PostData>(_mapper.ConfigurationProvider)
                .FirstOrDefaultAsyncEF();
            
            if (chapter == null) return NotFound();
            if (chapter.StoryAuthorId != User.GetNumericId()) return Unauthorized();

            Input = new PostData
            {
                Id = chapter.Id,
                Title = chapter.Title,
                Body = chapter.Body,
                StartNotes = chapter.StartNotes,
                EndNotes = chapter.EndNotes,
                IsPublished = chapter.IsPublished
            };
            
            return Page();
        }
        
        public class PostData
        {
            public long Id { get; init; }
            public string Title { get; init; }
            public string Body { get; init; }
            [Display(Name = "Start notes")]
            public string StartNotes { get; init; }
            [Display(Name = "End notes")]
            public string EndNotes { get; init; }
            public bool IsPublished { get; init; }
            public long StoryAuthorId { get; init; }
        }
        
        public class MappingProfile : Profile
        {
            public MappingProfile() => CreateMap<Chapter, PostData>();
        }
        
        public class PostDataValidation : AbstractValidator<PostData>
        {
            public PostDataValidation()
            {
                RuleFor(b => b.Title)
                    .NotEmpty()
                    .Length(CTConfig.CChapter.MinTitleLength, CTConfig.CChapter.MaxTitleLength);
                RuleFor(b => b.Body)
                    .NotEmpty()
                    .Length(CTConfig.CChapter.MinBodyLength, CTConfig.CChapter.MaxBodyLength);
                RuleFor(c => c.StartNotes)
                    .MaximumLength(CTConfig.CChapter.MaxNotesLength);
                RuleFor(c => c.EndNotes)
                    .MaximumLength(CTConfig.CChapter.MaxNotesLength);
                RuleFor(c => c.IsPublished)
                    .NotNull();
            }
        }

        public async Task<IActionResult> OnPostAsync(long id)
        {
            var uid = User.GetNumericId();
            if (uid is null) return Unauthorized();
            
            if (!ModelState.IsValid) return Page();

            var chapter = await _context.Chapters
                .Where(c => c.Id == id)
                .Where(c => c.Story.AuthorId == uid)
                .FirstOrDefaultAsyncEF();
            
            if (chapter is null) return NotFound();
            
            // Update the chapter
            chapter.Title       = Input.Title.Trim();
            chapter.Body        = Input.Body.Trim();
            chapter.StartNotes  = Input.StartNotes?.Trim();
            chapter.EndNotes    = Input.EndNotes?.Trim();
            chapter.Slug        = Input.Title.Trim().Friendlify();
            chapter.WordCount   = Input.Body.Words();
            chapter.IsPublished = Input.IsPublished;
            await _context.SaveChangesAsync();

            // NOTE: IDE will complain about non-nullable values, see https://github.com/linq2db/linq2db.EntityFrameworkCore/issues/135
            await _context.Stories
                .Where(s => s.Id == chapter.StoryId)
                .Set(s => s.WordCount, s => ((int?)s.Chapters
                    .Where(c => c.IsPublished)
                    .Sum(c => c.WordCount)) ?? 0)
                .Set(s => s.ChapterCount, s => s.Chapters.Count(c => c.IsPublished))
                .Set(s => s.IsPublished, s => s.Chapters.Any(c => c.IsPublished))
                .UpdateAsync();
            
            return RedirectToPage("../Chapter", new { id = chapter.Id, slug = chapter.Slug });
        }
    }
}
