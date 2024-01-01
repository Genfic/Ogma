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
using Ogma3.Data.Blogposts;
using Ogma3.Data.CommentsThreads;
using Ogma3.Data.Notifications;
using Ogma3.Infrastructure.CustomValidators;
using Ogma3.Infrastructure.Extensions;
using Ogma3.Pages.Shared.Minimals;
using Utils.Extensions;

namespace Ogma3.Pages.Blog;

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

	[BindProperty] public PostData Input { get; set; } = null!;

	public async Task<IActionResult> OnGet([FromQuery] long? story, [FromQuery] long? chapter)
	{
		Input = new PostData();

		if (story is not null)
		{
			Input.StoryMinimal = await _context.Stories
				.Where(s => s.Id == story)
				.Where(s => s.PublicationDate != null)
				.Where(b => b.ContentBlockId == null)
				.ProjectTo<StoryMinimal>(_mapper.ConfigurationProvider)
				.AsNoTracking()
				.FirstOrDefaultAsync();
			Input.StoryMinimalId = story;
			Input.IsUnavailable = Input.StoryMinimal is null;
		}
		else if (chapter is not null)
		{
			Input.ChapterMinimal = await _context.Chapters
				.Where(c => c.Id == chapter)
				.Where(c => c.PublicationDate != null)
				.Where(b => b.ContentBlockId == null)
				.ProjectTo<ChapterMinimal>(_mapper.ConfigurationProvider)
				.AsNoTracking()
				.FirstOrDefaultAsync();
			Input.ChapterMinimalId = chapter;
			Input.IsUnavailable = Input.ChapterMinimal is null;
		}

		return Page();
	}

	public class PostData
	{
		public string Title { get; init; } = null!;
		public string Body { get; init; } = null!;
		public string Tags { get; init; } = "";
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
		if (uid is null) return Unauthorized();

		// Create blogpost
		var post = new Blogpost
		{
			Title = Input.Title.Trim(),
			Slug = Input.Title.Trim().Friendlify(),
			Body = Input.Body.Trim(),
			AuthorId = (long)uid,
			WordCount = Input.Body.Words(),
			Hashtags = Input.Tags.ParseHashtags(),
			AttachedStoryId = Input.StoryMinimalId,
			AttachedChapterId = Input.ChapterMinimalId,
			CommentsThread = new CommentsThread(),
		};
		_context.Blogposts.Add(post);

		// Subscribe author to the comment thread
		_context.CommentsThreadSubscribers.Add(new CommentsThreadSubscriber
		{
			CommentsThread = post.CommentsThread,
			OgmaUserId = (long)uid
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