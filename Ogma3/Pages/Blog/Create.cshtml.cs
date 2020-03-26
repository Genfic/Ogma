using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Ogma3.Data;
using Ogma3.Data.Models;
using Utils;

namespace Ogma3.Pages.Blog
{
    public class CreateModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<User> _userManager;

        public CreateModel(ApplicationDbContext context, UserManager<User> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public IActionResult OnGet()
        {
            return Page();
        }

        [BindProperty]
        public InputModel Blogpost { get; set; }

        public class InputModel
        {
            [Required]
            [StringLength(CTConfig.Blogpost.MaxTitleLength,
                ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.",
                MinimumLength = CTConfig.Blogpost.MinTitleLength)]
            public string Title { get; set; }
            
            [Required]
            [StringLength(CTConfig.Blogpost.MaxBodyLength,
                ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.",
                MinimumLength = CTConfig.Blogpost.MinBodyLength)]
            public string Body { get; set; }
        }

        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }
            
            // Get logged in user
            var user = await _userManager.GetUserAsync(User);

            _context.Blogposts.Add(new Blogpost
            {
                Title = Blogpost.Title.Trim(),
                Slug = Blogpost.Title.Trim().Friendlify(),
                Body = Blogpost.Body.Trim(),
                Author = user
            });
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index", new { name = user.UserName });
        }
    }
}
