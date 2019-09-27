using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Ogma3.Data;

namespace Ogma3.Areas.Admin.Pages
{
    public class Tags : PageModel
    {
        [BindProperty]
        public InputModel Input { get; set; }

        public class InputModel
        {
            [Required]
            public string Name { get; set; }
            [Required]
            public string Description { get; set; }
        }
        
        public void OnGet()
        {
            
        }
    }
}