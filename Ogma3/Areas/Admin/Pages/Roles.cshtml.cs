using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Ogma3.Data.Models;

namespace Ogma3.Areas.Admin.Pages
{
    public class Roles : PageModel
    {
        [BindProperty]
        public OgmaRole Input { get; set; }
        public void OnGet()
        {
            
        }
    }
}