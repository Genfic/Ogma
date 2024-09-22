using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Ogma3.Data;
using Ogma3.Data.Infractions;
using Riok.Mapperly.Abstractions;

namespace Ogma3.Areas.Admin.Pages;

public sealed class InfractionsModel(ApplicationDbContext context) : PageModel
{
	public required List<InfractionDto> Infractions { get; set; }

	public async Task<IActionResult> OnGetAsync()
	{
		Infractions = await context.Infractions
			.OrderBy(i => i.IssueDate)
			.ToInfractionDtos()
			.ToListAsync();

		return Page();
	}
}

[Mapper(PreferParameterlessConstructors = false)]
public static partial class InfractionMapper
{
	public static partial IQueryable<InfractionDto> ToInfractionDtos(this IQueryable<Infraction> infraction);
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
		string RemovedByUserName,
		long RemovedById
	);