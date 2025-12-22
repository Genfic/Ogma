using System.ComponentModel.DataAnnotations;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Ogma3.Data;
using Ogma3.Data.Clubs;
using Ogma3.Data.Images;
using Ogma3.Infrastructure.CustomValidators;
using Ogma3.Infrastructure.CustomValidators.FileSizeValidator;
using Ogma3.Infrastructure.Extensions;
using Ogma3.Services.FileUploader;
using Routes.Pages;
using Utils.Extensions;

namespace Ogma3.Pages.Clubs;

[Authorize]
public sealed class CreateModel(ApplicationDbContext context, ImageUploader uploader, OgmaConfig ogmaConfig) : PageModel
{
	public IActionResult OnGet()
	{
		return Page();
	}

	[BindProperty] public required InputModel Input { get; set; }

	public sealed class InputModel
	{
		public required string Name { get; init; }
		public required string Hook { get; init; }
		public required string Description { get; init; }
		[DataType(DataType.Upload)] public IFormFile? Icon { get; init; }
	}

	public sealed class InputModelValidator : AbstractValidator<InputModel>
	{
		public InputModelValidator()
		{
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
				.FileHasExtension(CTConfig.ImageFileTypes);
		}
	}

	public async Task<IActionResult> OnPostAsync()
	{
		if (!ModelState.IsValid) return Page();

		if (User.GetNumericId() is not { } uid) return Unauthorized();

		var icon = "/img/placeholders/ph-250.png";
		string? iconId = null;
		if (Input.Icon is { Length: > 0})
		{
			var file = await uploader.Upload(
				Input.Icon,
				"club-icons",
				ogmaConfig.ClubIconWidth,
				ogmaConfig.ClubIconHeight
			);
			icon = Path.Join(ogmaConfig.Cdn, file.Path);
			iconId = file.FileId;
		}

		var club = new Data.Clubs.Club
		{
			Name = Input.Name,
			Slug = Input.Name.Friendlify(),
			Hook = Input.Hook,
			Description = Input.Description,
			Icon = new Image
			{
				Url = icon,
				BackblazeId = iconId,
			},
			ClubMembers =
			[
				new()
				{
					MemberId = uid,
					Role = EClubMemberRoles.Founder,
				},

			],
		};

		context.Clubs.Add(club);
		await context.SaveChangesAsync();

		return Club_Index.Get(club.Id, club.Slug).Redirect(this);
	}
}