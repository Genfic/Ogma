using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Ogma3.Data;
using Ogma3.Data.Chapters;
using Ogma3.Data.CommentsThreads;
using Ogma3.Data.Notifications;
using Ogma3.Data.Stories;
using Ogma3.Infrastructure.Extensions;
using Utils.Extensions;

namespace Ogma3.Pages.Chapters
{
    [Authorize]
    public class CreateModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly NotificationsRepository _notificationsRepo;
        private readonly IMapper _mapper;

        public CreateModel(ApplicationDbContext context, NotificationsRepository notificationsRepo, IMapper mapper)
        {
            _context = context;
            _notificationsRepo = notificationsRepo;
            _mapper = mapper;
        }

        public class GetData
        {
            public long Id { get; set; }
            public long? AuthorId { get; init; }
            public string Slug { get; init; }
            public string Title { get; init; }
        }
        
        public class MappingProfile : Profile
        {
            public MappingProfile() => CreateMap<Story, GetData>();
        }

        public async Task<IActionResult> OnGetAsync(long id)
        {
            Input = new PostData();

            Story = await _context.Stories
                .Where(s => s.Id == id)
                .ProjectTo<GetData>(_mapper.ConfigurationProvider)
                .FirstOrDefaultAsync();
            
            if (Story is null) return NotFound();
            if (Story.AuthorId != User.GetNumericId()) return RedirectToPage("../Story", new { id, slug = Story.Slug });
            
            return Page();
        }

        [BindProperty]
        public PostData Input { get; set; }
        public GetData Story { get; set; }

        public class PostData
        {
            public string Title { get; init; }
            public string Body { get; init; }
            [Display(Name = "Start notes")]
            public string StartNotes { get; init; }
            [Display(Name = "End notes")]
            public string EndNotes { get; init; }
        }

        public class PostDataValidation : AbstractValidator<PostData>
        {
            public PostDataValidation()
            {
                RuleFor(s => s.Title)
                    .NotEmpty()
                    .Length(CTConfig.CChapter.MinTitleLength, CTConfig.CChapter.MaxTitleLength);
                RuleFor(c => c.Body)
                    .NotEmpty()
                    .Length(CTConfig.CChapter.MinBodyLength, CTConfig.CChapter.MaxBodyLength);
                RuleFor(c => c.StartNotes)
                    .MaximumLength(CTConfig.CChapter.MaxNotesLength);
                RuleFor(c => c.EndNotes)
                    .MaximumLength(CTConfig.CChapter.MaxNotesLength);
            }
        }

        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync(long id)
        {
            if (!ModelState.IsValid) return Page();
            
            var uid = User.GetNumericId();
            if (uid is null) return Unauthorized();

            // Get the story to insert a chapter into. Include user in the search to check ownership.
            var story = await _context.Stories
                .Where(s => s.Id == id)
                .Where(s => s.AuthorId == uid)
                .Include(s => s.Chapters)
                .Include(s => s.Shelves.Where(x => x.TrackUpdates))
                .FirstOrDefaultAsync();

            // Back to index if the story is null or author isn't the logged in user
            if (story == null) return Page();
            
            // Get the order number of the latest chapter
            var latestChapter = story.Chapters
                .OrderByDescending(c => c.Order)
                .Select(c => c.Order)
                .FirstOrDefault();
            
            // Construct new chapter
            var chapter = new Chapter
            {
                Title = Input.Title.Trim(),
                Slug = Input.Title.Trim().Friendlify(),
                Body = Input.Body.Trim(),
                StartNotes = Input.StartNotes?.Trim(),
                EndNotes = Input.EndNotes?.Trim(),
                Order = latestChapter + 1,
                CommentsThread = new CommentsThread(),
                WordCount = Input.Body.Words()
            };
            
            // Recalculate words and chapters in the story
            story.WordCount = story.Chapters.Sum(c => c.WordCount) + chapter.WordCount;
            story.ChapterCount = story.Chapters.Count + 1;
            
            // Create the chapter and add it to the story
            story.Chapters.Add(chapter);       
            
            // Subscribe author to the comment thread
            await _context.CommentsThreadSubscribers.AddAsync(new CommentsThreadSubscriber
            {
                CommentsThread = chapter.CommentsThread,
                OgmaUserId = (long) uid
            });
            
            await _context.SaveChangesAsync();
            
            // Notify
            await _notificationsRepo.Create(ENotificationEvent.WatchedStoryUpdated,
                story.Shelves.Select(s => s.OwnerId),
                "/Chapter",
                new { chapter.Id, chapter.Slug });

            return RedirectToPage("../Chapter", new { id = chapter.Id, slug = chapter.Slug });
        }
    }
}
