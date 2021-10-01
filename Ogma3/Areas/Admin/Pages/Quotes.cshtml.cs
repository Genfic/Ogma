using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Ogma3.Areas.Admin.Pages;

[Authorize(Roles = "Admin")]
public class Quotes : PageModel
{
    public void OnGet()
    {
    }
}