using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Ogma3.Data;
using Ogma3.Data.Clubs;
using Ogma3.Infrastructure.CustomValidators;
using Ogma3.Infrastructure.CustomValidators.FileSizeValidator;
using Ogma3.Infrastructure.Extensions;
using Ogma3.Services.FileUploader;
using Serilog;
using Utils.Extensions;

namespace Ogma3.Pages.Clubs;

[Authorize]
public class EditModel : PageModel
{
    private readonly ApplicationDbContext _context;
    private readonly ImageUploader _uploader;
    private readonly OgmaConfig _ogmaConfig;

    public EditModel(ApplicationDbContext context, ImageUploader uploader, OgmaConfig ogmaConfig)
    {
        _context = context;
        _uploader = uploader;
        _ogmaConfig = ogmaConfig;
    }

    [BindProperty] 
    public InputModel Input { get; set; }

    public class InputModel
    {
        public long Id { get; init; }
        public string Name { get; init; }
        public string Slug { get; init; }
        public string Hook { get; init; }
        public string Description { get; init; }
        [DataType(DataType.Upload)] 
        public IFormFile Icon { get; init; }
    }

    public class InputModelValidator : AbstractValidator<InputModel>
    {
        public InputModelValidator()
        {
            RuleFor(m => m.Id)
                .NotEmpty();
            RuleFor(m => m.Name)
                .NotEmpty()
                .Length(CTConfig.CClub.MinNameLength, CTConfig.CClub.MaxNameLength);
            RuleFor(m => m.Hook)
                .NotEmpty()
                .Length(CTConfig.CClub.MinHookLength, CTConfig.CClub.MaxHookLength);
            RuleFor(m => m.Description)
                .MaximumLength(CTConfig.CClub.MaxDescriptionLength);
            RuleFor(m => m.Icon)
                .FileSmallerThan(CTConfig.CClub.CoverMaxWeight)
                .FileHasExtension(".jpg", ".jpeg", ".png", ".webp");
        }
    }

    public async Task<IActionResult> OnGetAsync(long? id)
    {
        if (id is null) return NotFound();

        var uid = User.GetNumericId();
        if (uid is null) return Unauthorized();

        Input = await _context.Clubs
            .Where(c => c.Id == id)
            .Where(c => c.ClubMembers
                .Where(cm => cm.MemberId == uid)
                .Any(cm => cm.Role == EClubMemberRoles.Founder || cm.Role == EClubMemberRoles.Admin))
            .Select(c => new InputModel
            {
                Id = c.Id,
                Name = c.Name,
                Slug = c.Slug,
                Hook = c.Hook,
                Description = c.Description
            })
            .AsNoTracking()
            .FirstOrDefaultAsync();

        if (Input is null) return NotFound();

        return Page();
    }
        
    public async Task<IActionResult> OnPostAsync(long? id)
    {
        if (!ModelState.IsValid) return Page();

        var uid = User.GetNumericId();
        if (uid is null) return Unauthorized();
        
        Log.Information("User {UserId} attempted to edit club {ClubId}", uid, id);

        var club = await _context.Clubs
            .Where(c => c.Id == id)
            .Where(c => c.ClubMembers
                .Where(cm => cm.MemberId == uid)
                .Any(cm => cm.Role == EClubMemberRoles.Founder || cm.Role == EClubMemberRoles.Admin))
            .FirstOrDefaultAsync();
        if (club is null)
        {
            Log.Information("User {UserId} did not succeed in editing club {ClubId}", uid, id);
            return NotFound();
        }

        club.Name = Input.Name;
        club.Slug = Input.Name.Friendlify();
        club.Hook = Input.Hook;
        club.Description = Input.Description;

        Log.Information("User {UserId} succeeded in editing club {ClubId}", uid, id);
        await _context.SaveChangesAsync();

        if (Input.Icon is not { Length: > 0 }) return RedirectToPage("/Club/Index", new { club.Id, club.Slug });
            
        var file = await _uploader.Upload(
            Input.Icon,
            "club-icons",
            club.Id.ToString(),
            _ogmaConfig.ClubIconWidth,
            _ogmaConfig.ClubIconHeight
        );
        club.IconId = file.FileId;
        club.Icon = Path.Join(_ogmaConfig.Cdn, file.Path);
            
        // Final save
        await _context.SaveChangesAsync();

        return RedirectToPage("/Club/Index", new {club.Id, club.Slug});
    }
}