using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Ogma3.Areas.Admin.Pages;

public class Ratings : PageModel
{
	[BindProperty] public InputModel Input { get; init; }

	public class InputModel
	{
		public string Name { get; init; }
		public string Description { get; init; }
		public byte Order { get; init; }
		public IFormFile Icon { get; init; }
		public bool BlacklistedByDefault { get; init; }
	}

	public void OnGet()
	{
	}
}