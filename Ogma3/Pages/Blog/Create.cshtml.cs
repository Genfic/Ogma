using System;
using System.Linq;
using System.Threading.Tasks;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Ogma3.Data;
using Ogma3.Data.Blogposts;
using Ogma3.Data.Chapters;
using Ogma3.Data.CommentsThreads;
using Ogma3.Data.Notifications;
using Ogma3.Data.Stories;
using Ogma3.Infrastructure.CustomValidators;
using Ogma3.Infrastructure.Extensions;
using Ogma3.Pages.Shared.Minimals;
using Utils.Extensions;

namespace Ogma3.Pages.Blog
{
    [Authorize]
    public class CreateModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly StoriesRepository _storiesRepo;
        private readonly ChaptersRepository _chaptersRepo;
        private readonly NotificationsRepository _notificationsRepo;

        public CreateModel(ApplicationDbContext context, StoriesRepository storiesRepo, ChaptersRepository chaptersRepo, NotificationsRepository notificationsRepo)
        {
            _context = context;
            _storiesRepo = storiesRepo;
            _chaptersRepo = chaptersRepo;
            _notificationsRepo = notificationsRepo;
        }

        [BindProperty]
        public PostData Input { get; set; }

        public async Task<IActionResult> OnGet([FromQuery] long? story, [FromQuery] long? chapter)
        {
            Input = new PostData();

            if (story is not null)
            {
                Input.StoryMinimal = await _storiesRepo.GetMinimal((long) story);
                Input.StoryMinimalId = story;
                Input.IsUnavailable = Input.StoryMinimal is null;
            }
            else if (chapter is not null)
            {
                Input.ChapterMinimal = await _chaptersRepo.GetMinimal((long) chapter);
                Input.ChapterMinimalId = chapter;
                Input.IsUnavailable = Input.ChapterMinimal is null;
            }
            
            return Page();
        }

        public class PostData
        {
            public string Title { get; init; }
            public string Body { get; init; }
            public string Tags { get; init; }
            public ChapterMinimal? ChapterMinimal { get; set; }
            public long? ChapterMinimalId { get; set; }
            public StoryMinimal? StoryMinimal { get; set; }
            public long? StoryMinimalId { get; set; }
            public bool IsUnavailable { get; set; }
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
        
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return await OnGet(Input.StoryMinimalId, Input.ChapterMinimalId);
            }
            
            // Get logged in user
            var uid = User.GetNumericId();
            var uname = User.GetUsername();
            
            // Return if not logged in
            if (uid is null || uname is null) return Unauthorized();
            
            // Create blogpost
            var post = new Blogpost
            {
                Title = Input.Title.Trim(),
                Slug = Input.Title.Trim().Friendlify(),
                Body = Input.Body.Trim(),
                AuthorId = (long) uid,
                WordCount = Input.Body.Words(),
                Hashtags = Input.Tags?.ParseHashtags() ?? Array.Empty<string>(),
                AttachedStoryId = Input.StoryMinimalId,
                AttachedChapterId = Input.ChapterMinimalId,
                CommentsThread = new CommentsThread(),
            };
            _context.Blogposts.Add(post);

            // Subscribe author to the comment thread
            _context.CommentsThreadSubscribers.Add(new CommentsThreadSubscriber
            {
                CommentsThread = post.CommentsThread,
                OgmaUserId = (long) uid
            });

            await _context.SaveChangesAsync();

            // Notify followers
            var notificationRecipients = await _context.Users
                .Where(u => u.Following.Any(a => a.Id == uid))
                .Select(u => u.Id)
                .ToListAsync();
            await _notificationsRepo.Create(ENotificationEvent.FollowedAuthorNewBlogpost,
                notificationRecipients,
                "/Blog/Post",
                new { post.Id, post.Slug });

            return RedirectToPage("/User/Blog", new { name = uname });
        }
    }
}
