using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Ogma3.Data;
using Ogma3.Data.Bases;
using Ogma3.Data.Blogposts;
using Ogma3.Data.Chapters;
using Ogma3.Data.ModeratorActions;
using Ogma3.Data.Stories;
using Ogma3.Infrastructure.Constants;
using Ogma3.Infrastructure.Extensions;
using Ogma3.Infrastructure.ServiceRegistrations;

namespace Ogma3.Areas.Admin.Pages;

[Authorize(AuthorizationPolicies.RequireAdminOrModeratorRole)]
public sealed class ContentBlock(ApplicationDbContext context) : PageModel
{
	[BindProperty] public ItemType Type { get; set; }

	[BindProperty] public long Id { get; set; }

	public ItemData? Item { get; private set; }

	public async Task<ActionResult> OnGet(ItemType type, long id)
	{
		Type = type;
		Id = id;

		Item = type switch
		{
			ItemType.Story => await context.Stories.Where(s => s.Id == id)
				.Select(s => new ItemData
				{
					Blocked = s.ContentBlockId != null,
					Reason = s.ContentBlock == null ? null : s.ContentBlock.Reason,
					Title = s.Title,
					Type = nameof(Story),
				})
				.FirstOrDefaultAsync(),
			ItemType.Chapter => await context.Chapters.Where(c => c.Id == id)
				.Select(c => new ItemData
				{
					Blocked = c.ContentBlockId != null,
					Reason = c.ContentBlock == null ? null : c.ContentBlock.Reason,
					Title = c.Title,
					Subtitle = c.Story.Title,
					Type = nameof(Chapter),
				})
				.FirstOrDefaultAsync(),
			ItemType.Blogpost => await context.Blogposts.Where(b => b.Id == id)
				.Select(b => new ItemData
				{
					Blocked = b.ContentBlockId != null,
					Reason = b.ContentBlock == null ? null : b.ContentBlock.Reason,
					Title = b.Title,
					Type = nameof(Blogpost),
				})
				.FirstOrDefaultAsync(),
			_ => null,
		};

		return Page();
	}

	public async Task<ActionResult> OnPostBlock([FromForm] PostData data)
	{
		if (data.Reason.Length < 20)
		{
			TempData["error"] = "Reason has to be at least 20 characters long";
			return Routes.Areas.Admin.Pages.ContentBlock.Get(Type, Id).Redirect(this);
		}

		if (User.GetNumericId() is not { } staffId) return Unauthorized();
		_ = Type switch
		{
			ItemType.Story => await TryBlockContent<Story>(Id, data.Reason, staffId),
			ItemType.Chapter => await TryBlockContent<Chapter>(Id, data.Reason, staffId),
			ItemType.Blogpost => await TryBlockContent<Blogpost>(Id, data.Reason, staffId),
			_ => false,
		};

		return Routes.Areas.Admin.Pages.ContentBlock.Get(Type, Id).Redirect(this);
	}

	private async Task<bool> TryBlockContent<T>(long itemId, string reason, long uid) where T : BaseModel, IBlockableContent
	{
		if (User.GetNumericId() is not { } staffId) return false;
		if (User.GetUsername() is not { } uname) return false;

		var item = await context.Set<T>()
			.Where(i => i.Id == itemId)
			.Include(i => i.ContentBlock)
			.FirstOrDefaultAsync();
		if (item is null) return false;

		var title = item switch
		{
			Story s => s.Title,
			Chapter c => c.Title,
			Blogpost b => b.Title,
			_ => "",
		};

		if (item.ContentBlock is not null) return true;

		item.ContentBlock = new Data.Blacklists.ContentBlock
		{
			Reason = reason,
			IssuerId = uid,
			Type = typeof(T).Name,
		};

		// Log the action
		context.ModeratorActions.Add(new ModeratorAction
		{
			StaffMemberId = staffId,
			Description = ModeratorActionTemplates.ContentBlocked(Type.ToString(), title, itemId, uname),
		});

		await context.SaveChangesAsync();
		return true;
	}

	public async Task<ActionResult> OnPostUnblock()
	{
		_ = Type switch
		{
			ItemType.Story => await TryUnblockContent<Story>(Id),
			ItemType.Chapter => await TryUnblockContent<Chapter>(Id),
			ItemType.Blogpost => await TryUnblockContent<Blogpost>(Id),
			_ => false,
		};

		return Routes.Areas.Admin.Pages.ContentBlock.Get(Type, Id).Redirect(this);
	}

	private async Task<bool> TryUnblockContent<T>(long itemId) where T : BaseModel, IBlockableContent
	{
		if (User.GetNumericId() is not { } staffId) return false;
		if (User.GetUsername() is not { } uname) return false;

		var item = await context.Set<T>()
			.Where(i => i.Id == itemId)
			.Include(i => i.ContentBlock)
			.FirstOrDefaultAsync();
		if (item is null) return false;

		var title = item switch
		{
			Story s => s.Title,
			Chapter c => c.Title,
			Blogpost b => b.Title,
			_ => "",
		};

		if (item.ContentBlock is null) return true;

		item.ContentBlock = null;

		// Log the action
		context.ModeratorActions.Add(new ModeratorAction
		{
			StaffMemberId = staffId,
			Description = ModeratorActionTemplates.ContentUnblocked(Type.ToString(), title, itemId, uname),
		});

		await context.SaveChangesAsync();
		return true;
	}

	public enum ItemType
	{
		Blogpost,
		Story,
		Chapter,
	}

	public sealed record PostData(string Reason = "");

	public sealed class ItemData
	{
		public required bool Blocked { get; init; }
		public required string? Reason { get; init; }
		public required string Title { get; init; }
		public string? Subtitle { get; init; }
		public required string Type { get; init; }
	}
}