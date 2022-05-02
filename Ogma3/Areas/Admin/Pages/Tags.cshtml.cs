using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Ogma3.Data.Tags;

namespace Ogma3.Areas.Admin.Pages;

public class Tags : PageModel
{
	[BindProperty] public InputModel Input { get; set; }

	public class InputModel
	{
		[Required] public string Name { get; set; }
		[Required] public string Description { get; set; }

		public ETagNamespace Namespace { get; set; }
	}

	public void OnGet()
	{
	}
}