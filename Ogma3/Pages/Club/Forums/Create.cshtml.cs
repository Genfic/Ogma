using System;
using System.Threading.Tasks;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Ogma3.Data;
using Ogma3.Data.Clubs;
using Ogma3.Data.ClubThreads;
using Ogma3.Data.CommentsThreads;
using Ogma3.Infrastructure.Extensions;

namespace Ogma3.Pages.Club.Forums;

public class CreateModel(ApplicationDbContext context, ClubRepository clubRepo) : PageModel
{
	[BindProperty] public required PostModel ClubThread { get; set; }

	public async Task<IActionResult> OnGet(long id)
	{
		var uid = User.GetNumericId();

		if (!await clubRepo.IsMember(uid, id)) return Unauthorized();

		ClubThread = new PostModel
		{
			Title = "",
			Body = "",
			ClubId = id
		};
		return Page();
	}

	public class PostModel
	{
		public required string Title { get; init; }
		public required string Body { get; init; }
		public required long ClubId { get; init; }
	}

	public class PostModelValidator : AbstractValidator<PostModel>
	{
		public PostModelValidator()
		{
			RuleFor(p => p.Title)
				.NotEmpty()
				.Length(CTConfig.CClubThread.MinTitleLength, CTConfig.CClubThread.MaxTitleLength);
			RuleFor(p => p.Body)
				.NotEmpty()
				.Length(CTConfig.CClubThread.MinBodyLength, CTConfig.CClubThread.MaxBodyLength);
			RuleFor(p => p.ClubId)
				.NotEmpty();
		}
	}

	public async Task<IActionResult> OnPostAsync()
	{
		if (!ModelState.IsValid) return Page();

		if (User.GetNumericId() is not {} uid) return Unauthorized();

		if (!await clubRepo.IsMember(uid, ClubThread.ClubId)) return Unauthorized();

		var clubThread = new ClubThread
		{
			AuthorId = uid,
			Title = ClubThread.Title,
			Body = ClubThread.Body,
			ClubId = ClubThread.ClubId,
			CreationDate = DateTime.Now,
			CommentsThread = new CommentsThread()
		};

		context.ClubThreads.Add(clubThread);
		await context.SaveChangesAsync();

		return RedirectToPage("Details", new { threadId = clubThread.Id, clubId = clubThread.ClubId });
	}
}