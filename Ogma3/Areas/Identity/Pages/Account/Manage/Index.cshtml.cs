using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;
using FluentValidation;
using MemoryPack;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Ogma3.Data;
using Ogma3.Data.Images;
using Ogma3.Data.Users;
using Ogma3.Infrastructure.CustomValidators;
using Ogma3.Infrastructure.CustomValidators.FileSizeValidator;
using Ogma3.Infrastructure.Extensions;
using Ogma3.Services.FileUploader;
using Utils.Extensions;
using ZiggyCreatures.Caching.Fusion;

namespace Ogma3.Areas.Identity.Pages.Account.Manage;

public sealed partial class IndexModel
(
	ApplicationDbContext context,
	SignInManager<OgmaUser> signInManager,
	ImageUploader uploader,
	OgmaConfig config,
	IFusionCache cache) : PageModel
{
	[TempData] public string StatusMessage { get; set; } = "";
	[BindProperty] public required InputModel Input { get; set; }

	private List<TimezoneEntry> _availableTimezones = [];
	public List<SelectListItem> AvailableTimezones => _availableTimezones.Select(tz => new SelectListItem(tz.Text, tz.Value)).ToList();

	public sealed class InputModel
	{
		[DataType(DataType.Upload)] public IFormFile? Avatar { get; init; }
		public bool DeleteAvatar { get; init; }
		public string? Title { get; init; }
		public string? Bio { get; init; }
		public string? Links { get; init; }
		public string? Timezone { get; set; }
	}

	public sealed class InputModelValidation : AbstractValidator<InputModel>
	{
		public InputModelValidation()
		{
			RuleFor(x => x.Avatar!)
				.FileSmallerThan(CTConfig.Files.AvatarMaxWeight)
				.FileHasExtension(".jpg", ".jpeg", ".png")
				.When(x => x.Avatar is not null);
			RuleFor(x => x.Title)
				.MaximumLength(CTConfig.User.MaxTitleLength);
			RuleFor(x => x.Bio)
				.MaximumLength(CTConfig.User.MaxBioLength);
			RuleFor(x => x.Links)
				.MaximumLines(CTConfig.User.MaxLinksAmount);
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
				Timezone = u.TimeZone,
			})
			.FirstOrDefaultAsync();
	}

	public async Task<IActionResult> OnGetAsync()
	{
		if (User.GetNumericId() is not {} uid) return Unauthorized();

		var model = await LoadAsync(uid);
		if (model is null) return NotFound();
		Input = model;

		_availableTimezones = cache.GetOrSet(
			"AvailableTimezones",
			TimeZoneInfo.GetSystemTimeZones()
				.Select(tzi => {
					if (tzi.HasIanaId)
					{
						return new TimezoneEntry(tzi.Id, tzi.DisplayName);
					}

					return TimeZoneInfo.TryConvertWindowsIdToIanaId(tzi.Id, out var ianaId)
						? new TimezoneEntry(ianaId, tzi.DisplayName)
						: null;
				})
				.OfType<TimezoneEntry>()
				.OrderBy(i => i.Text)
				.ToList(),
			opt => opt.Duration = TimeSpan.FromSeconds(3)
		);

		return Page();
	}

	public async Task<IActionResult> OnPostAsync()
	{
		if (User.GetNumericId() is not {} uid) return Unauthorized();

		var user = await context.Users
			.Where(u => u.Id == uid)
			.Include(u => u.Avatar)
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
			if (user.Avatar.BackblazeId is not null)
			{
				await uploader.Delete(user.Avatar.Url, user.Avatar.BackblazeId);
			}

			// Upload the new one
			var file = await uploader.Upload(
				Input.Avatar,
				"avatars",
				config.AvatarWidth,
				config.AvatarHeight
			);
			user.Avatar = new Image
			{
				Url = Path.Join(config.Cdn, file.Path),
				BackblazeId = file.FileId,
			};
		}
		else if (Input.DeleteAvatar)
		{
			if (user.Avatar.BackblazeId is not null)
			{
				await uploader.Delete(user.Avatar.Url, user.Avatar.BackblazeId);
			}

			user.Avatar = new Image
			{
				Url = new Uri(config.AvatarServiceUrl).AppendSegments($"{user.UserName}.png").ToString(),
				BackblazeId = null,
			};
		}

		if (Input.Title != user.Title)
		{
			user.Title = Input.Title;
		}

		if (Input.Bio != user.Bio)
		{
			user.Bio = Input.Bio;
		}

		if (Input.Timezone is not null && Input.Timezone != user.TimeZone)
		{
			user.TimeZone = Input.Timezone;
		}

		if (Input.Links is {} links)
		{
			user.Links.Clear();
			foreach (var link in links.AsSpan().EnumerateLines())
			{
				var trimmed = link.Trim();
				if (trimmed.IsEmpty) continue;
				var line = link.ToString();

				if (Uri.TryCreate(line, UriKind.RelativeOrAbsolute, out _))
				{
					user.Links.Add(UrlRegex.Match(line).Groups[1].Value);
				}
			}
		}

		await context.SaveChangesAsync();

		await signInManager.RefreshSignInAsync(user);
		StatusMessage = "Your profile has been updated";
		return RedirectToPage();
	}

	[MemoryPackable]
	private sealed partial record TimezoneEntry(string Value, string Text);

	[GeneratedRegex("(?:https|http)?(?:://)?(?:w{3}\\.)?(.+)")]
	private partial Regex UrlRegex { get; }
}