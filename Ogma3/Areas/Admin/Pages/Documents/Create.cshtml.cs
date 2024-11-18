using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Ogma3.Data;
using Ogma3.Data.Documents;
using Utils.Extensions;

namespace Ogma3.Areas.Admin.Pages.Documents;

public sealed class CreateModel : PageModel
{
	private readonly ApplicationDbContext _context;

	public CreateModel(ApplicationDbContext context)
	{
		_context = context;
	}

	[BindProperty] public required InputModel Input { get; set; }

	public sealed class InputModel
	{
		public required long Id { get; set; }

		[Required] public required string Title { get; set; }

		[Required] public required string Body { get; set; }
	}

	public void OnGet()
	{
	}

	public async Task<IActionResult> OnPostAsync()
	{
		_context.Documents.Add(new Document
		{
			Title = Input.Title,
			Slug = Input.Title.Friendlify(),
			Body = Input.Body,
			Version = 1,
			CreationTime = DateTimeOffset.UtcNow,
			RevisionDate = null,
		});

		await _context.SaveChangesAsync();
		return Routes.Pages.Index.Get().Redirect(this);
	}
}