using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Ogma3.Data;
using Ogma3.Data.ClubModeratorActions;
using Ogma3.Data.Clubs;
using Ogma3.Infrastructure.Constants;
using Ogma3.Infrastructure.Extensions;

namespace Ogma3.Pages.Club.Forums;

[Authorize]
public sealed class DeleteModel(ApplicationDbContext context) : PageModel
{
	[BindProperty] public required GetData ClubThread { get; set; }

	public sealed class GetData
	{
		public required long Id { get; init; }
		public required long ClubId { get; init; }
		public required long AuthorId { get; init; }
		public required string Title { get; init; }
		public required DateTimeOffset CreationDate { get; init; }
		public required int Replies { get; init; }
	}

	public async Task<IActionResult> OnGetAsync(long id)
	{
		var clubThread = await context.ClubThreads
			.TagWith($"Clubs — Delete — {nameof(OnGetAsync)}")
			.Where(ct => ct.Id == id)
			.Select(ct => new GetData
			{
				Id = ct.Id,
				ClubId = ct.ClubId,
				AuthorId = ct.AuthorId,
				Title = ct.Title,
				CreationDate = ct.CreationDate,
				Replies = ct.CommentThread.CommentsCount,
			})
			.FirstOrDefaultAsync();

		if (clubThread is null) return NotFound();
		ClubThread = clubThread;

		var (allowed, _) = await CanDelete(ClubThread.AuthorId, ClubThread.ClubId);
		
		if (!allowed) return Unauthorized();

		return Page();
	}

	public async Task<IActionResult> OnPostAsync(long id)
	{
		if (User.GetNumericId() is not {} uid) return Unauthorized();
		
		var th = await context.ClubThreads
			.TagWith($"Clubs — Delete — {nameof(OnPostAsync)} — get threads")
			.Where(ct => ct.Id == id)
			.Select(ct => new
			{
				ct.Id,
				ct.AuthorId,
				ct.ClubId,
				ct.Club.Slug,
				ct.Title,
			})
			.FirstOrDefaultAsync();

		if (th is null) return NotFound();
		
		var (allowed, isModerator) = await CanDelete(th.AuthorId, th.ClubId);
		
		if (!allowed) return Unauthorized();
		
		if (isModerator)
		{
			context.ClubModeratorActions.Add(new ClubModeratorAction
			{
				ModeratorId = uid,
				ClubId = th.ClubId,
				Description = ModeratorActionTemplates.ForumThreadDeleted(th.Title, th.Id, User.GetUsername() ?? "[unknown]"),
			});
		}

		await context.ClubThreads
			.TagWith($"Clubs — Delete — {nameof(OnPostAsync)} — update delete time")
			.Where(ct => ct.Id == th.Id)
			.ExecuteUpdateAsync(setters => setters.SetProperty(ct => ct.DeletedAt, DateTimeOffset.UtcNow));

		return Routes.Pages.Club_Forums_Index.Get(th.ClubId, th.Slug).Redirect(this);
	}

	private async Task<(bool allowed, bool isModerator)> CanDelete(long? authorId, long clubId)
	{
		// User is not even logged in
		if (User.GetNumericId() is not {} uid) return (false, false);

		// User is logged in and is the thread's author
		if (authorId == uid) return (true, false);

		var isModerator = await context.ClubMembers
			.TagWith($"Clubs — Delete — {nameof(CanDelete)}")
			.Where(cm => cm.ClubId == clubId)
			.Where(cm => cm.MemberId == uid)
			.Where(cm => new[]
			{
				EClubMemberRoles.Founder,
				EClubMemberRoles.Admin,
				EClubMemberRoles.Moderator,
			}.Contains(cm.Role))
			.AnyAsync();

		// The user might or might not be a moderator
		return (isModerator, isModerator);
	}
}