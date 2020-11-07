using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Ogma3.Data;
using Utils.Extensions;

namespace Ogma3.Pages.Club.Forums
{
    [Authorize]
    public class EditModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public EditModel(ApplicationDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public async Task<IActionResult> OnGetAsync(long id)
        {

            Input = await _context.ClubThreads
                .Where(ct => ct.Id == id)
                .Where(ct => ct.AuthorId == User.GetNumericId())
                .Select(ct => new InputModel
                {
                    Id = ct.Id,
                    Title = ct.Title,
                    Body = ct.Body,
                })
                .FirstOrDefaultAsync();

            if (Input == null) return NotFound();
            
            return Page();
        }
        
        public class InputModel
        {
            public long Id { get; set; }
            
            [Required]
            [MinLength(CTConfig.CClubThread.MinTitleLength)]
            [MaxLength(CTConfig.CClubThread.MaxTitleLength)]
            public string Title { get; set; }

            [Required]
            [MinLength(CTConfig.CClubThread.MinBodyLength)]
            [MaxLength(CTConfig.CClubThread.MaxBodyLength)]
            public string Body { get; set; }
        }

        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid) return Page();

            var clubThread = await _context.ClubThreads
                .Where(ct => ct.Id == Input.Id)
                .Where(ct => ct.AuthorId == User.GetNumericId())
                .FirstOrDefaultAsync();
            
            if (clubThread == null) return NotFound();

            clubThread.Title = Input.Title;
            clubThread.Body = Input.Body;

            await _context.SaveChangesAsync();
            
            return RedirectToPage("./Details", new { clubId = clubThread.ClubId, threadId = clubThread.Id });
        }
    }
}
