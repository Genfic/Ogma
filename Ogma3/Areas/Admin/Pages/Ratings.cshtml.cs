using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Ogma3.Data.Ratings;

namespace Ogma3.Areas.Admin.Pages
{
    public class Ratings : PageModel
    {
        [BindProperty]
        public Rating Input { get; set; }
        
        public void OnGet()
        {
            
        }
    }
}