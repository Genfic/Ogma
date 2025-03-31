using System.ComponentModel.DataAnnotations;
using FluentValidation;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Ogma3.Data;
using Ogma3.Data.Users;
using Ogma3.Infrastructure.CustomValidators;
using Ogma3.Infrastructure.CustomValidators.FileSizeValidator;
using Ogma3.Infrastructure.Extensions;
using Ogma3.Services.FileUploader;
using Utils.Extensions;

namespace Ogma3.Areas.Identity.Pages.Account.Manage;

public sealed class IndexModel(
	ApplicationDbContext context,
	SignInManager<OgmaUser> signInManager,
	ImageUploader uploader,
	OgmaConfig config) : PageModel
{
	[TempData] public string StatusMessage { get; set; } = "";
	[BindProperty] public required InputModel Input { get; set; }

	public sealed class InputModel
	{
		[DataType(DataType.Upload)] public IFormFile? Avatar { get; init; }
		public bool DeleteAvatar { get; init; }
		public string? Title { get; init; }
		public string? Bio { get; init; }
		public string? Links { get; init; }
	}

	public sealed class InputModelValidation : AbstractValidator<InputModel>
	{
		public InputModelValidation()
		{
			RuleFor(x => x.Avatar!)
				.FileSmallerThan(CTConfig.CFiles.AvatarMaxWeight)
				.FileHasExtension(".jpg", ".jpeg", ".png")
				.When(x => x.Avatar is not null);
			RuleFor(x => x.Title)
				.MaximumLength(CTConfig.CUser.MaxTitleLength);
			RuleFor(x => x.Bio)
				.MaximumLength(CTConfig.CUser.MaxBioLength);
			RuleFor(x => x.Links)
				.MaximumLines(CTConfig.CUser.MaxLinksAmount);
		}
	}

	private async Task<InputModel?> LoadAsync(long? uid)
	{
		return await context.Users
			.Where(u => u.Id == uid)
			.Select(u => new InputModel
			{
				Title = u.Title,
				Bio = u.Bio,
				Links = string.Join('\n', u.Links),
			})
			.FirstOrDefaultAsync();
	}

	public async Task<IActionResult> OnGetAsync()
	{
		if (User.GetNumericId() is not {} uid) return Unauthorized();

		var model = await LoadAsync(uid);
		if (model is null) return NotFound();
		Input = model;

		return Page();
	}

	public async Task<IActionResult> OnPostAsync()
	{
		if (User.GetNumericId() is not {} uid) return Unauthorized();

		var user = await context.Users
			.Where(u => u.Id == uid)
			.FirstOrDefaultAsync();

		if (user is null) return NotFound("Unable to load user");

		if (!ModelState.IsValid)
		{
			var model = await LoadAsync(uid);
			if (model is null) return NotFound();
			Input = model;
			return Page();
		}

		// If new avatar provided, replace the old one
		if (Input.Avatar is { Length: > 0 })
		{
			// Delete the old avatar if exists
			if (user.AvatarId is not null)
			{
				await uploader.Delete(user.Avatar, user.AvatarId);
			}

			// Upload the new one
			var file = await uploader.Upload(
				Input.Avatar,
				"avatars",
				config.AvatarWidth,
				config.AvatarHeight
			);
			user.AvatarId = file.FileId;
			user.Avatar = Path.Join(config.Cdn, file.Path);
		}
		else if (Input.DeleteAvatar)
		{
			if (user.AvatarId is not null)
			{
				await uploader.Delete(user.Avatar, user.AvatarId);
			}

			user.AvatarId = null;
			user.Avatar = new Uri(config.AvatarServiceUrl).AppendSegments($"{user.UserName}.png").ToString();
		}

		if (Input.Title != user.Title)
		{
			user.Title = Input.Title;
		}

		if (Input.Bio != user.Bio)
		{
			user.Bio = Input.Bio;
		}

		user.Links = Input.Links?
			.Split(["\r\n", "\r", "\n"], StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
			.Where(l => Uri.TryCreate(l, UriKind.RelativeOrAbsolute, out _))
			.ToList() ?? [];

		await context.SaveChangesAsync();

		await signInManager.RefreshSignInAsync(user);
		StatusMessage = "Your profile has been updated";
		return RedirectToPage();
	}
}