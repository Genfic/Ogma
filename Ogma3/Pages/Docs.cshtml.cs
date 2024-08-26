using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Ogma3.Data;
using Utils.Extensions;
using String = Utils.Extensions.String;

namespace Ogma3.Pages;

public class Docs(ApplicationDbContext context) : PageModel
{
	public required DocumentDto Document { get;  set; }
	public required List<DocumentVersionDto> Versions { get; set; }
	public required IEnumerable<String.Header> Headers { get; set; }

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
				Body = d.Body,
				Version = d.Version,
			})
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

		Headers = Document.Body.GetMarkdownHeaders();

		return Page();
	}
	
	public class DocumentDto
	{
		public required string Title { get; init; }
		public required string Slug { get; init; }
		public required string Body { get; init; }
		public required uint Version { get; init; }
	}

	public class DocumentVersionDto
	{
		public required string Slug { get; init; }
		public required uint Version { get; init; }
		public required DateTime CreationTime { get; init; }
	}
}