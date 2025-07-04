using System.ComponentModel.DataAnnotations;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Ogma3.Data;
using Ogma3.Data.Clubs;
using Ogma3.Data.Images;
using Ogma3.Infrastructure.CustomValidators;
using Ogma3.Infrastructure.CustomValidators.FileSizeValidator;
using Ogma3.Infrastructure.Extensions;
using Ogma3.Services.FileUploader;
using Utils.Extensions;

namespace Ogma3.Pages.Clubs;

[Authorize]
public sealed class EditModel(ApplicationDbContext context, ImageUploader uploader, OgmaConfig ogmaConfig, ILogger<EditModel> logger) : PageModel
{
	[BindProperty] public required InputModel Input { get; set; }

	public sealed class InputModel
	{
		public required long Id { get; init; }
		public required string Name { get; init; }
		public required string Slug { get; init; }
		public required string Hook { get; init; }
		public required string Description { get; init; }
		[DataType(DataType.Upload)]public IFormFile? Icon { get; init; }
	}

	public sealed class InputModelValidator : AbstractValidator<InputModel>
	{
		public InputModelValidator()
		{
			RuleFor(m => m.Id)
				.NotEmpty();
			RuleFor(m => m.Name)
				.NotEmpty()
				.Length(CTConfig.Club.MinNameLength, CTConfig.Club.MaxNameLength);
			RuleFor(m => m.Hook)
				.NotEmpty()
				.Length(CTConfig.Club.MinHookLength, CTConfig.Club.MaxHookLength);
			RuleFor(m => m.Description)
				.MaximumLength(CTConfig.Club.MaxDescriptionLength);
			RuleFor(m => m.Icon)
				.FileSmallerThan(CTConfig.Club.CoverMaxWeight)
				.FileHasExtension(".jpg", ".jpeg", ".png", ".webp");
		}
	}

	public async Task<IActionResult> OnGetAsync(long id)
	{
		var uid = User.GetNumericId();
		if (uid is null) return Unauthorized();

		var input = await context.Clubs
			.Where(c => c.Id == id)
			.Where(c => c.ClubMembers
				.Where(cm => cm.MemberId == uid)
				.Any(cm => cm.Role == EClubMemberRoles.Founder || cm.Role == EClubMemberRoles.Admin))
			.Select(c => new InputModel
			{
				Id = c.Id,
				Name = c.Name,
				Slug = c.Slug,
				Hook = c.Hook,
				Description = c.Description,
			})
			.AsNoTracking()
			.FirstOrDefaultAsync();

		if (input is null) return NotFound();

		Input = input;

		return Page();
	}

	public async Task<IActionResult> OnPostAsync(long? id)
	{
		if (!ModelState.IsValid) return Page();

		if (User.GetNumericId() is not { } uid) return Unauthorized();

		logger.LogInformation("User {UserId} attempted to edit club {ClubId}", uid, id);

		var club = await context.Clubs
			.Where(c => c.Id == id)
			.Where(c => c.ClubMembers
				.Where(cm => cm.MemberId == uid)
				.Any(cm => cm.Role == EClubMemberRoles.Founder || cm.Role == EClubMemberRoles.Admin))
			.FirstOrDefaultAsync();

		if (club is null)
		{
			logger.LogInformation("User {UserId} did not have the right role to update club {ClubId}, or club does not exist", uid, id);
			return NotFound();
		}

		club.Name = Input.Name;
		club.Slug = Input.Name.Friendlify();
		club.Hook = Input.Hook;
		club.Description = Input.Description;

		if (Input.Icon is { Length: > 0 })
		{
			var file = await uploader.Upload(
				Input.Icon,
				"club-icons",
				ogmaConfig.ClubIconWidth,
				ogmaConfig.ClubIconHeight
			);
			club.Icon = new Image
			{
				Url = Path.Join(ogmaConfig.Cdn, file.Path),
				BackblazeId = file.FileId,
			};
		}

		logger.LogInformation("User {UserId} succeeded in editing club {ClubId}", uid, id);
		await context.SaveChangesAsync();

		return Routes.Pages.Club_Index.Get(club.Id, club.Slug).Redirect(this);
	}
}