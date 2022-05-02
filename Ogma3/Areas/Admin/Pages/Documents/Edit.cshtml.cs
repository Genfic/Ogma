using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Ogma3.Data;
using Ogma3.Data.Documents;
using Ogma3.Infrastructure.Constants;

namespace Ogma3.Areas.Admin.Pages.Documents;

[Authorize(Roles = RoleNames.Admin)]
public class EditModel : PageModel
{
	private readonly ApplicationDbContext _context;

	public EditModel(ApplicationDbContext context)
	{
		_context = context;
	}

	[BindProperty] public InputModel Input { get; set; }

	public class InputModel
	{
		[Required] public string Slug { get; set; }

		[Required] public string Title { get; set; }

		[Required] public string Body { get; set; }
	}

	public Document Doc { get; set; }

	public async Task<IActionResult> OnGetAsync(string slug)
	{
		Doc = await _context.Documents
			.Where(d => d.Slug == slug)
			.Where(d => d.RevisionDate == null)
			.AsNoTracking()
			.FirstOrDefaultAsync();

		if (Doc == null) return NotFound();

		Input = new InputModel
		{
			Slug = Doc.Slug,
			Title = Doc.Title,
			Body = Doc.Body
		};
		return Page();
	}

	public async Task<IActionResult> OnPostAsync()
	{
		var oldVersion = await _context.Documents
			.Where(d => d.RevisionDate == null)
			.Where(d => d.Slug == Input.Slug)
			.FirstOrDefaultAsync();

		if (oldVersion is null) return Page();

		var now = DateTime.Now;

		_context.Documents.Add(new Document
		{
			Title = oldVersion.Title,
			Slug = oldVersion.Slug,
			Body = string.IsNullOrEmpty(Input.Body) ? oldVersion.Body : Input.Body,
			Version = oldVersion.Version + 1,
			CreationTime = now,
			RevisionDate = null
		});

		oldVersion.RevisionDate = now;

		await _context.SaveChangesAsync();
		return RedirectToPage("./Index");
	}
}