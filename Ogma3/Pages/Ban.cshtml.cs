using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Ogma3.Data;
using Ogma3.Data.Infractions;
using Ogma3.Infrastructure.Attributes;
using Ogma3.Infrastructure.Extensions;
using Index = Routes.Pages.Index;

namespace Ogma3.Pages;

[AllowBannedUsers]
public sealed class Ban(ApplicationDbContext context) : PageModel
{
	public DateTimeOffset BannedUntil { get; private set; }
	public List<InfractionDto> Infractions { get; private set; } = [];

	public async Task<IActionResult> OnGetAsync()
	{
		var uid = User.GetNumericId();
		if (uid is null) return Unauthorized();

		Infractions = await context.Infractions
			.Where(i => i.UserId == uid)
			.Where(i => i.Type != InfractionType.Note)
			.OrderByDescending(i => i.Type)
			.ThenByDescending(i => i.ActiveUntil)
			.Select(i => new InfractionDto
			{
				IssueDate = i.IssueDate,
				ActiveUntil = i.ActiveUntil,
				RemovedAt = i.RemovedAt,
				Reason = i.Reason,
				Type = i.Type,
			})
			.ToListAsync();

		BannedUntil = Infractions
			.Where(i => i.RemovedAt == null)
			.Where(i => i.ActiveUntil > DateTimeOffset.UtcNow)
			.OrderByDescending(i => i.ActiveUntil)
			.Select(i => i.ActiveUntil)
			.FirstOrDefault();

		if (BannedUntil == default)
		{
			return Index.Get().Redirect(this);
		}

		return Page();
	}

	public sealed record InfractionDto
	{
		public required DateTimeOffset IssueDate { get; init; }
		public required DateTimeOffset ActiveUntil { get; init; }
		public required DateTimeOffset? RemovedAt { get; init; }
		public required string Reason { get; init; }
		public required InfractionType Type { get; init; }
	}
}