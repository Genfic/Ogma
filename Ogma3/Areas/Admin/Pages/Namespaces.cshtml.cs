using System.ComponentModel.DataAnnotations;
using System.Drawing;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Ogma3.Data.Models;

namespace Ogma3.Areas.Admin.Pages
{
    public class Namespaces : PageModel
    {
        [BindProperty]
        public Namespace Input { get; set; }
        
        public void OnGet()
        {
            
        }
    }
}