using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Ogma3.Data;
using Ogma3.Data.GlobalNotifications;
using Ogma3.Infrastructure.Extensions;
using Riok.Mapperly.Abstractions;
using ZiggyCreatures.Caching.Fusion;

namespace Ogma3.Areas.Admin.Pages;

public sealed class NotificationsModel(ApplicationDbContext context, IFusionCache cache) : PageModel
{
	public required List<NotificationDto> Notifications { get; set; }

	[BindProperty]
	public required long? Id { get; set; }

	public async Task<IActionResult> OnGetAsync([FromQuery] long? id = null)
	{
		Id = id;

		if (id is not null)
		{
			var notif = await context.GlobalNotifications
				.Where(n => n.Id == id)
				.Select(n => new { n.Message, n.Color })
				.FirstOrDefaultAsync();

			Message = notif?.Message;
			Color = $"#{notif?.Color}";
		}

		Notifications = await context.GlobalNotifications
			.ProjectToDto()
			.ToListAsync();

		return Page();
	}

	[BindProperty]
	public string? Message { get; set; }

	[BindProperty]
	public string? Color { get; set; }

	public async Task<IActionResult> OnPostAsync()
	{
		if (User.GetNumericId() is not {} uid)
		{
			return Unauthorized();
		}

		if (Message is null)
		{
			return Page();
		}

		if (Id is null)
		{
			var notif = new GlobalNotification
			{
				Message = Message,
				Color = Color,
				CreatedById = uid,
			};

			context.GlobalNotifications.Add(notif);
			await context.SaveChangesAsync();
		}
		else
		{
			await context.GlobalNotifications
				.Where(n => n.Id == Id)
				.ExecuteUpdateAsync(s => s
					.SetProperty(n => n.Message, Message)
					.SetProperty(n => n.Color, Color?.Trim().TrimStart('#')));
		}

		await ClearCache();

		return Routes.Areas.Admin.Pages.Notifications.Get().Redirect(this);
	}

	public async Task<IActionResult> OnGetArchive(long id)
	{
		await context.GlobalNotifications
			.Where(n => n.Id == id)
			.ExecuteUpdateAsync(s => s
				.SetProperty(n => n.ArchivedAt, DateTimeOffset.UtcNow));

		await ClearCache();

		return Routes.Areas.Admin.Pages.Notifications.Get().Redirect(this);
	}

	private async Task ClearCache()
	{
		await cache.ExpireAsync("global-notifications");
	}
}

public sealed record NotificationDto (
	long? Id,
	string Message,
	DateTimeOffset CreatedAt,
	DateTimeOffset? ArchivedAt,
	DateTimeOffset? ExpiresAt,
	string? Color,
	string CreatedByUserName
);

[Mapper]
public static partial class GlobalNotifMapper
{
	public static partial IQueryable<NotificationDto> ProjectToDto(this IQueryable<GlobalNotification> query);
}