using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Ogma3.Data;
using Ogma3.Data.Clubs;
using Ogma3.Infrastructure.Extensions;
using Ogma3.Services.FileUploader;

namespace Ogma3.Pages.Clubs;

[Authorize]
public sealed class DeleteModel(ApplicationDbContext context, ImageUploader uploader, ILogger<DeleteModel> logger) : PageModel
{
	[BindProperty] public required GetData Club { get; set; }

	public sealed class GetData
	{
		public required long Id { get; init; }
		public required string Name { get; init; }
		public required string Slug { get; init; }
		public required string Hook { get; init; }
		public required DateTimeOffset CreationDate { get; init; }
	}

	public async Task<IActionResult> OnGetAsync(long? id)
	{
		if (id is null) return NotFound();

		var uid = User.GetNumericId();
		if (uid is null) return Unauthorized();

		var club = await context.Clubs
			.Where(c => c.Id == id)
			.Where(c => c.ClubMembers
				.Where(cm => cm.MemberId == uid)
				.Any(cm => cm.Role == EClubMemberRoles.Founder || cm.Role == EClubMemberRoles.Admin))
			.Select(c => new GetData
			{
				Id = c.Id,
				Name = c.Name,
				Slug = c.Slug,
				Hook = c.Hook,
				CreationDate = c.CreationDate,
			})
			.AsNoTracking()
			.FirstOrDefaultAsync();

		if (club is null) return NotFound();

		Club = club;

		return Page();
	}

	public async Task<IActionResult> OnPostAsync(long? id)
	{
		if (id is null) return NotFound();

		var uid = User.GetNumericId();
		if (uid is null) return Unauthorized();

		logger.LogInformation("User {UserId} attempted to delete club {ClubId}", uid, id);

		var icon = await context.Clubs
			.Where(c => c.Id == id)
			.Select(c => c.Icon)
			.FirstOrDefaultAsync();

		var rows = await context.Clubs
			.Where(c => c.ClubMembers
				.Where(cm => cm.MemberId == uid)
				.Any(cm => cm.Role == EClubMemberRoles.Founder || cm.Role == EClubMemberRoles.Admin))
			.ExecuteDeleteAsync();

		if (rows <= 0)
		{
			logger.LogInformation("User {UserId} did not succeed in deleting club {ClubId}", uid, id);
			return NotFound();
		}

		logger.LogInformation("User {UserId} succeeded in deleting club {ClubId}", uid, id);

		if (icon is { BackblazeId: not null })
		{
			await uploader.Delete(icon.Url, icon.BackblazeId);
		}

		var imageRows = await context.Images
			.Where(i => i.Id == (icon == null ? null : icon.Id))
			.ExecuteDeleteAsync();

		if (imageRows <= 0)
		{
			logger.LogInformation("User {UserId} did not succeed in deleting club icon {ClubId}", uid, id);
		}

		return Routes.Pages.Index.Get().Redirect(this);
	}
}