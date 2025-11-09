using System.Linq.Expressions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Ogma3.Data;
using Ogma3.Data.Infractions;

namespace Ogma3.Areas.Admin.Pages;

public sealed class InfractionsModel(ApplicationDbContext context) : PageModel
{
	public required List<InfractionDto> Infractions { get; set; }

	public async Task<IActionResult> OnGetAsync()
	{
		Infractions = await context.Infractions
			.OrderBy(i => i.IssueDate)
			.Select(InfractionMapper.ToInfractionDto)
			.ToListAsync();

		return Page();
	}
}

public static class InfractionMapper
{
	public static readonly Expression<Func<Infraction, InfractionDto>> ToInfractionDto = i => new InfractionDto(
		i.User.UserName,
		i.UserId,
		i.IssueDate,
		i.ActiveUntil,
		i.RemovedAt,
		i.Reason,
		i.Type,
		i.IssuedBy.UserName,
		i.IssuedById,
		i.RemovedBy == null ? null : i.RemovedBy.UserName,
		i.RemovedById
	);
}

public record InfractionDto(
		string UserUserName,
		long UserId,
		DateTimeOffset IssueDate,
		DateTimeOffset ActiveUntil,
		DateTimeOffset? RemovedAt,
		string Reason,
		InfractionType Type,
		string IssuedByUserName,
		long IssuedById,
		string? RemovedByUserName,
		long? RemovedById
	);