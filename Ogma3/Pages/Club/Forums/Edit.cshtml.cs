using System.Linq;
using System.Threading.Tasks;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Ogma3.Data;
using Ogma3.Infrastructure.Extensions;

namespace Ogma3.Pages.Club.Forums;

[Authorize]
public class EditModel : PageModel
{
    private readonly ApplicationDbContext _context;

    public EditModel(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> OnGetAsync(long id)
    {
        var uid = User.GetNumericId();
        if (uid is null) return Unauthorized();
            
        Input = await _context.ClubThreads
            .Where(ct => ct.Id == id)
            .Where(ct => ct.AuthorId == uid)
            .Select(ct => new InputModel
            {
                Id = ct.Id,
                ClubId = ct.ClubId,
                Title = ct.Title,
                Body = ct.Body
            })
            .FirstOrDefaultAsync();

        if (Input is null) return NotFound();
            
        return Page();
    }

    [BindProperty]
    public InputModel Input { get; set; }
        
    public class InputModel
    {
        public long Id { get; init; }
        public long ClubId { get; init; }
        public string Title { get; init; }
        public string Body { get; init; }
    }
        
    public class InputModelValidator : AbstractValidator<InputModel>
    {
        public InputModelValidator()
        {
            RuleFor(m => m.Id)
                .NotEmpty();
            RuleFor(m => m.ClubId)
                .NotEmpty();
            RuleFor(m => m.Title)
                .NotEmpty()
                .Length(CTConfig.CClubThread.MinTitleLength, CTConfig.CClubThread.MaxTitleLength);
            RuleFor(m => m.Body)
                .NotEmpty()
                .Length(CTConfig.CClubThread.MinBodyLength, CTConfig.CClubThread.MaxBodyLength);
        }
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid) return Page();

        var uid = User.GetNumericId();
        if (uid is null) return Unauthorized();

        var clubThread = await _context.ClubThreads
            .Where(ct => ct.Id == Input.Id)
            .FirstOrDefaultAsync();
            
        if (clubThread is null) return NotFound();
        if (clubThread.AuthorId != uid) return Unauthorized();
            
        clubThread.Title = Input.Title;
        clubThread.Body = Input.Body;

        await _context.SaveChangesAsync();
            
        return RedirectToPage("./Details", new { clubId = clubThread.ClubId, threadId = clubThread.Id });
    }
}