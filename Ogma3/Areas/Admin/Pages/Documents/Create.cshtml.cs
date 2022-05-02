using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Ogma3.Data;
using Ogma3.Data.Documents;
using Utils.Extensions;

namespace Ogma3.Areas.Admin.Pages.Documents;

public class CreateModel : PageModel
{
	private readonly ApplicationDbContext _context;

	public CreateModel(ApplicationDbContext context)
	{
		_context = context;
	}

	[BindProperty] public InputModel Input { get; set; }

	public class InputModel
	{
		public long Id { get; set; }

		[Required] public string Title { get; set; }

		[Required] public string Body { get; set; }
	}

	public void OnGetAsync()
	{
	}

	public async Task<IActionResult> OnPostAsync()
	{
		var now = DateTime.Now;

		_context.Documents.Add(new Document
		{
			Title = Input.Title,
			Slug = Input.Title.Friendlify(),
			Body = Input.Body,
			Version = 1,
			CreationTime = now,
			RevisionDate = null
		});

		await _context.SaveChangesAsync();
		return RedirectToPage("./Index");
	}
}