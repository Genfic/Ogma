using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Ogma3.Data;
using Ogma3.Data.Models;

namespace Ogma3.Pages.Club.Forums
{
    public class CreateModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly OgmaUserManager _userManager;

        public CreateModel(ApplicationDbContext context, OgmaUserManager userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        [BindProperty]
        public PostModel ClubThread { get; set; }

        public IActionResult OnGet(long id)
        {
            ClubThread = new PostModel
            {
                ClubId = id
            };
            return Page();
        }

        public class PostModel
        {
            [Required]
            [MinLength(CTConfig.CClubThread.MinTitleLength)]
            [MaxLength(CTConfig.CClubThread.MaxTitleLength)]
            public string Title { get; set; }
        
            [Required]
            [MinLength(CTConfig.CClubThread.MinBodyLength)]
            [MaxLength(CTConfig.CClubThread.MaxBodyLength)]
            public string Body { get; set; }
        
            [Required]
            public long ClubId { get; set; }
        }
        
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }
            
            var user = await _userManager.GetUserAsync(User);
            var clubThread = new ClubThread
            {
                Author = user,
                Title = ClubThread.Title,
                Body = ClubThread.Body,
                ClubId = ClubThread.ClubId,
                CreationDate = DateTime.Now,
                CommentsThread = new CommentsThread()
            };

            await _context.ClubThreads.AddAsync(clubThread);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index", new { id = ClubThread.ClubId });
        }
    }
}
