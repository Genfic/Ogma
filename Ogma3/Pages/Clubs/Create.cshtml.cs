using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Ogma3.Data;
using Ogma3.Data.Enums;
using Ogma3.Data.Models;

namespace Ogma3.Pages.Clubs
{
    public class CreateModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private OgmaUserManager _userManager;

        public CreateModel(ApplicationDbContext context, OgmaUserManager userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public IActionResult OnGet()
        {
            return Page();
        }

        [BindProperty]
        public Data.Models.Club Club { get; set; }

        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var currentUser = await _userManager.GetUserAsync(User);
            
            Club.ClubMembers.Add(new ClubMember
            {
                Member = currentUser,
                Role = EClubMemberRoles.Founder,
                MemberSince = DateTime.Now
            });

            await _context.Clubs.AddAsync(Club);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}
