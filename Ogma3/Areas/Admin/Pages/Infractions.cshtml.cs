using System.Linq.Expressions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Ogma3.Data;
using Ogma3.Data.Infractions;
using Ogma3.Infrastructure.ServiceRegistrations;
using Ogma3.Pages.Shared;

namespace Ogma3.Areas.Admin.Pages;

[Authorize(AuthorizationPolicies.RequireAdminOrModeratorRole)]
public sealed class InfractionsModel(ApplicationDbContext context) : PageModel
{
	private const int PerPage = 50;

	public required List<InfractionDto> Infractions { get; set; }
	public required Pagination Pagination { get; set; }

	public async Task OnGetAsync([FromQuery] int page = 1)
	{
		Infractions = await context.Infractions
			.OrderBy(i => i.IssueDate)
			.Select(InfractionMapper.ToInfractionDto)
			.Paginate(page, PerPage)
			.ToListAsync();
		var count = await context.Infractions.CountAsync();

		Pagination = new()
		{
			CurrentPage = page,
			ItemCount =	count,
			PerPage = PerPage,
		};
	}
}

public static class InfractionMapper
{
	public static readonly Expression<Func<Infraction, InfractionDto>> ToInfractionDto = i => new InfractionDto(
		i.User.UserName,
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

public sealed record InfractionDto(
		string UserUserName,
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