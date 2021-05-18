using System;
using System.Threading.Tasks;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Ogma3.Data;
using Ogma3.Data.Clubs;
using Ogma3.Data.CommentsThreads;
using Ogma3.Infrastructure.Extensions;

namespace Ogma3.Pages.Club.Forums
{
    public class CreateModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public CreateModel(ApplicationDbContext context)
        {
            _context = context;
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
            public string Title { get; init; }
            public string Body { get; init; } 
            public long ClubId { get; init; }
        }
        
        public class PostModelValidator : AbstractValidator<PostModel>
        {
            public PostModelValidator()
            {
                RuleFor(p => p.Title)
                    .NotEmpty()
                    .Length(CTConfig.CClubThread.MinTitleLength, CTConfig.CClubThread.MaxTitleLength);
                RuleFor(p => p.Body)
                    .NotEmpty()
                    .Length(CTConfig.CClubThread.MinBodyLength, CTConfig.CClubThread.MaxBodyLength);
                RuleFor(p => p.ClubId)
                    .NotEmpty();
            }
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid) return Page();
            
            // Get logged in user
            var uid = User.GetNumericId();
            if (uid is null) return Unauthorized();
            
            var clubThread = new ClubThread
            {
                AuthorId = (long)uid,
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
