using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Ogma3.Data;
using Ogma3.Data.Documents;

namespace Ogma3.Pages;

public sealed class Docs(ApplicationDbContext context) : PageModel
{
	public required DocumentDto Document { get;  set; }
	public required List<DocumentVersionDto> Versions { get; set; }

	public async Task<IActionResult> OnGetAsync(string slug, [FromQuery] uint? v)
	{
		var query = context.Documents
			.Where(d => d.Slug == slug);

		query = v.HasValue
			? query.Where(d => d.Version == v)
			: query.OrderByDescending(d => d.Version);

		var document = await query
			.Select(d => new DocumentDto
			{
				Title = d.Title,
				Slug = d.Slug,
				CompiledBody = d.CompiledBody,
				Version = d.Version,
				Headers = d.Headers,
			})
			.AsNoTracking()
			.FirstOrDefaultAsync();

		if (document is null) return NotFound();
		Document = document;

		Versions = await context.Documents
			.Where(d => d.Slug == Document.Slug)
			.OrderByDescending(d => d.Version)
			.Select(d => new DocumentVersionDto
			{
				Slug = d.Slug,
				Version = d.Version,
				CreationTime = d.CreationTime,
			})
			.ToListAsync();

		return Page();
	}

	public sealed class DocumentDto
	{
		public required string Title { get; init; }
		public required string Slug { get; init; }
		public required uint Version { get; init; }
		public required string CompiledBody { get; init; }
		public required List<Document.Header> Headers { get; init; }
	}

	public sealed class DocumentVersionDto
	{
		public required string Slug { get; init; }
		public required uint Version { get; init; }
		public required DateTimeOffset CreationTime { get; init; }
	}
}