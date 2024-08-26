using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Ogma3.Data;
using Ogma3.Infrastructure.Extensions;

namespace Ogma3.Pages.Club.Forums;

[Authorize]
public class EditModel(ApplicationDbContext context) : PageModel
{
	public async Task<IActionResult> OnGetAsync(long id)
	{
		if (User.GetNumericId() is not { } uid) return Unauthorized();

		var input = await context.ClubThreads
			.Where(ct => ct.Id == id)
			.Where(ct => ct.AuthorId == uid)
			.Select(ct => new InputModel
			{
				Id = ct.Id,
				ClubId = ct.ClubId,
				Title = ct.Title,
				Body = ct.Body,
			})
			.FirstOrDefaultAsync();

		if (input is null) return NotFound();
		Input = input;
		
		return Page();
	}

	[BindProperty] public required InputModel Input { get; set; }

	public class InputModel
	{
		public required long Id { get; init; }
		public required long ClubId { get; init; }
		public required string Title { get; init; }
		public required string Body { get; init; }
	}

	public class InputModelValidator : AbstractValidator<InputModel>
	{
		public InputModelValidator()
		{
			RuleFor(m => m.Id)
				.NotEmpty();
			RuleFor(m => m.ClubId)
				.NotEmpty();
			RuleFor(m => m.Title)
				.NotEmpty()
				.Length(CTConfig.CClubThread.MinTitleLength, CTConfig.CClubThread.MaxTitleLength);
			RuleFor(m => m.Body)
				.NotEmpty()
				.Length(CTConfig.CClubThread.MinBodyLength, CTConfig.CClubThread.MaxBodyLength);
		}
	}

	public async Task<IActionResult> OnPostAsync()
	{
		if (!ModelState.IsValid) return Page();

		if (User.GetNumericId() is not { } uid) return Unauthorized();

		var clubThread = await context.ClubThreads
			.Where(ct => ct.Id == Input.Id)
			.FirstOrDefaultAsync();

		if (clubThread is null) return NotFound();
		if (clubThread.AuthorId != uid) return Unauthorized();

		clubThread.Title = Input.Title;
		clubThread.Body = Input.Body;

		await context.SaveChangesAsync();

		return RedirectToPage("./Details", new { clubId = clubThread.ClubId, threadId = clubThread.Id });
	}
}