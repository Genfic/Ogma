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
using Routes.Pages;
using Utils.Extensions;

namespace Ogma3.Pages.Blog;

[Authorize]
public sealed class CreateModel(ApplicationDbContext context, NotificationsRepository notificationsRepo)
	: PageModel
{
	[BindProperty] public required PostData Input { get; set; }

	public async Task<IActionResult> OnGet([FromQuery] long? story, [FromQuery] long? chapter)
	{
		Input = new PostData();

		if (story is not null)
		{
			Input.StoryMinimal = await context.Stories
				.Where(s => s.Id == story)
				.Where(s => s.PublicationDate != null)
				.Where(b => b.ContentBlockId == null)
				.ProjectToMinimal()
				.FirstOrDefaultAsync();
			Input.StoryMinimalId = story;
			Input.IsUnavailable = Input.StoryMinimal is null;
		}
		else if (chapter is not null)
		{
			Input.ChapterMinimal = await context.Chapters
				.Where(c => c.Id == chapter)
				.Where(c => c.PublicationDate != null)
				.Where(b => b.ContentBlockId == null)
				.ProjectToMinimal()
				.FirstOrDefaultAsync();
			Input.ChapterMinimalId = chapter;
			Input.IsUnavailable = Input.ChapterMinimal is null;
		}

		return Page();
	}

	public sealed class PostData
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

	public sealed class PostDataValidation : AbstractValidator<PostData>
	{
		public PostDataValidation()
		{
			RuleFor(b => b.Title)
				.NotEmpty()
				.Length(CTConfig.Blogpost.MinTitleLength, CTConfig.Blogpost.MaxTitleLength);
			RuleFor(b => b.Body)
				.NotEmpty()
				.Length(CTConfig.Blogpost.MinBodyLength, CTConfig.Blogpost.MaxBodyLength);
			RuleFor(b => b.Tags)
				.HashtagsFewerThan(CTConfig.Blogpost.MaxTagsAmount)
				.HashtagsShorterThan(CTConfig.Blogpost.MaxTagLength);
		}
	}

	public async Task<IActionResult> OnPostAsync()
	{
		if (!ModelState.IsValid)
		{
			return await OnGet(Input.StoryMinimalId, Input.ChapterMinimalId);
		}

		// Get the logged-in user
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
			AuthorId = (long)uid,
			WordCount = Input.Body.Words(),
			Hashtags = Input.Tags.ParseHashtags().ToArray(),
			AttachedStoryId = Input.StoryMinimalId,
			AttachedChapterId = Input.ChapterMinimalId,
			CommentThread = new CommentThread(),
		};
		context.Blogposts.Add(post);

		// Subscribe author to the comment thread
		context.CommentThreadSubscribers.Add(new CommentThreadSubscriber
		{
			CommentThread = post.CommentThread,
			OgmaUserId = (long)uid,
		});

		await context.SaveChangesAsync();

		// Notify followers
		var notificationRecipients = await context.Users
			.Where(u => u.Following.Any(a => a.Id == uid))
			.Select(u => u.Id)
			.ToListAsync();
		await notificationsRepo.Create(ENotificationEvent.FollowedAuthorNewBlogpost,
			notificationRecipients,
			"/Blog/Post",
			new { post.Id, post.Slug });

		return User_Blog.Get(uname).Redirect(this);
	}
}