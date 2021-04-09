using System;
using System.Linq;
using System.Threading.Tasks;
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
using Ogma3.Infrastructure.Extensions;
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
        public BlogpostCreateDto Input { get; set; }

        public async Task<IActionResult> OnGet([FromQuery] long? story, [FromQuery] long? chapter)
        {
            await Init(story, chapter);
            return Page();
        }


        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                await Init(Input.StoryMinimalId, Input.ChapterMinimalId);
                return Page();
            }
            
            // Get logged in user
            var uid = User.GetNumericId();
            var uname = User.GetUsername();
            
            // Return if not logged in
            if (uid == null || uname == null) return Unauthorized();
            
            // Create array of hashtags
            var tags = Input.Tags?
                .Split(',')
                .Where(t => !string.IsNullOrWhiteSpace(t))
                .ToList()
                .Select(t => t.Trim(' ', '#', ',').Friendlify())
                .Distinct()
                .ToArray();

            var post = new Blogpost
            {
                Title = Input.Title.Trim(),
                Slug = Input.Title.Trim().Friendlify(),
                Body = Input.Body.Trim(),
                AuthorId = (long) uid,
                WordCount = Input.Body.Trim().Split(' ', '\t', '\n').Length,
                Hashtags = tags ?? Array.Empty<string>(),
                AttachedStoryId = Input.StoryMinimalId,
                AttachedChapterId = Input.ChapterMinimalId,
                CommentsThread = new CommentsThread(),
            };

            await _context.Blogposts.AddAsync(post);

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

        private async Task Init(long? story, long? chapter)
        {
            Input = new BlogpostCreateDto();

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
        }
    }
}
