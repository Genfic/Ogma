using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Ogma3.Data;
using Ogma3.Data.Documents;
using Ogma3.Infrastructure.ServiceRegistrations;
using Riok.Mapperly.Abstractions;

namespace Ogma3.Areas.Admin.Pages.Documents;

[Authorize(AuthorizationPolicies.RequireStaffRole)]
public sealed class IndexModel(ApplicationDbContext context) : PageModel
{
	public required List<DocumentDto> Docs { get; set; }

	public async Task OnGetAsync()
	{
		Docs = await context.Documents
			.Where(d => !d.RevisionDate.HasValue)
			.OrderBy(d => d.Slug)
			.MapToDto()
			.ToListAsync();
	}
}

public sealed record DocumentDto(
	string Title,
	string Slug,
	DateTimeOffset? RevisionDate,
	DateTimeOffset CreationTime,
	uint Version
)
{
	public sealed record HeaderDto(byte Level, string Id, string Body);
}

[Mapper]
public static partial class DocumentMapper
{
	public static partial IQueryable<DocumentDto> MapToDto(this IQueryable<Document> doc);
}