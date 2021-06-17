using System.ComponentModel.DataAnnotations;
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
using Ogma3.Infrastructure.Extensions;
using Ogma3.Services.FileUploader;
using Utils.Extensions;

namespace Ogma3.Pages.Clubs
{
    [Authorize]
    public class EditModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly ImageUploader _uploader;
        private readonly OgmaConfig _config;

        public EditModel(ApplicationDbContext context, ImageUploader uploader, OgmaConfig config)
        {
            _context = context;
            _uploader = uploader;
            _config = config;
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
            public long FounderId { get; init; }
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
                    .FileHasExtension(new[] {".jpg", ".jpeg", ".png", ".webp"});
            }
        }

        public async Task<IActionResult> OnGetAsync(long? id)
        {
            if (id is null) return NotFound();

            var uid = User.GetNumericId();
            if (uid is null) return Unauthorized();

            Input = await _context.Clubs
                .Where(c => c.Id == id)
                .Select(c => new InputModel
                {
                    Id = c.Id,
                    Name = c.Name,
                    Slug = c.Slug,
                    Hook = c.Hook,
                    Description = c.Description,
                    FounderId = c.ClubMembers.FirstOrDefault(cm => cm.Role == EClubMemberRoles.Founder).MemberId
                })
                .AsNoTracking()
                .FirstOrDefaultAsync();

            if (Input is null) return NotFound();
            if (Input.FounderId != uid) return Unauthorized();

            return Page();
        }
        
        public async Task<IActionResult> OnPostAsync(long? id)
        {
            if (!ModelState.IsValid) return Page();

            var uid = User.GetNumericId();
            if (uid is null) return Unauthorized();

            var club = await _context.Clubs
                .Where(c => c.Id == id)
                .FirstOrDefaultAsync();
            if (club is null) return NotFound();

            var authorized = await _context.ClubMembers
                .Where(cm => cm.ClubId == id)
                .Where(cm => cm.MemberId == uid)
                .Where(cm => cm.Role == EClubMemberRoles.Founder)
                .AnyAsync();
            if (!authorized) return Unauthorized();
                
            club.Name = Input.Name;
            club.Slug = Input.Name.Friendlify();
            club.Hook = Input.Hook;
            club.Description = Input.Description;

            await _context.SaveChangesAsync();
            
            if (Input.Icon is {Length: > 0})
            {
                var file = await _uploader.Upload(
                    Input.Icon,
                    "club-icons",
                    $"{club.Id}-{club.Name.Friendlify()}",
                    _config.ClubIconWidth,
                    _config.ClubIconHeight
                );
                club.IconId = file.FileId;
                club.Icon = file.Path;
                // Final save
                await _context.SaveChangesAsync();
            }

            return RedirectToPage("/Club/Index", new {club.Id, club.Slug});
        }
    }
}