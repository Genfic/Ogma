using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Ogma3.Data;
using Ogma3.Data.Documents;
using Ogma3.Infrastructure.Constants;

namespace Ogma3.Areas.Admin.Pages.Documents;

[Authorize(Roles = RoleNames.Admin)]
public sealed class EditModel : PageModel
{
	private readonly ApplicationDbContext _context;

	public EditModel(ApplicationDbContext context)
	{
		_context = context;
	}

	[BindProperty] public required InputModel Input { get; set; }

	public sealed class InputModel
	{
		[Required] public required string Slug { get; set; }

		[Required] public required string Title { get; set; }

		[Required] public required string Body { get; set; }
		public uint Version { get; set; }
	}
	
	public async Task<IActionResult> OnGetAsync(string slug)
	{
		var doc = await _context.Documents
			.Where(d => d.Slug == slug)
			.Where(d => d.RevisionDate == null)
			.Select(d => new InputModel
			{
				Slug = d.Slug,
				Title = d.Title,
				Body = d.Body,
				Version = d.Version,
			})
			.FirstOrDefaultAsync();

		if (doc is null) return NotFound();

		Input = doc;
		
		return Page();
	}

	public async Task<IActionResult> OnPostAsync()
	{
		var oldVersion = await _context.Documents
			.Where(d => d.RevisionDate == null)
			.Where(d => d.Slug == Input.Slug)
			.FirstOrDefaultAsync();

		if (oldVersion is null) return Page();

		var now = DateTimeOffset.UtcNow;

		_context.Documents.Add(new Document
		{
			Title = oldVersion.Title,
			Slug = oldVersion.Slug,
			Body = string.IsNullOrEmpty(Input.Body) ? oldVersion.Body : Input.Body,
			Version = oldVersion.Version + 1,
			CreationTime = now,
			RevisionDate = null,
		});

		oldVersion.RevisionDate = now;

		await _context.SaveChangesAsync();
		return Routes.Pages.Index.Get().Redirect(this);
	}
}