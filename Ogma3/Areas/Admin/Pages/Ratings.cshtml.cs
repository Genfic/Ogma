using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Ogma3.Areas.Admin.Pages;

public sealed class Ratings : PageModel
{
	[BindProperty]
	public required InputModel Input { get; init; }

	public sealed class InputModel
	{
		public required string Name { get; init; }
		public required string Description { get; init; }
		public required byte Order { get; init; }
		public required IFormFile Icon { get; init; }
		public required bool BlacklistedByDefault { get; init; }
	}

	public void OnGet()
	{
	}
}