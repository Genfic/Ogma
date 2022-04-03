using System;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using FluentValidation;
using Flurl;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Ogma3.Data;
using Ogma3.Data.Users;
using Ogma3.Infrastructure.CustomValidators;
using Ogma3.Infrastructure.CustomValidators.FileSizeValidator;
using Ogma3.Infrastructure.Extensions;
using Ogma3.Services.FileUploader;

namespace Ogma3.Areas.Identity.Pages.Account.Manage;

public class IndexModel : PageModel
{
    private readonly ApplicationDbContext _context;
    private readonly SignInManager<OgmaUser> _signInManager;
    private readonly ImageUploader _uploader;
    private readonly OgmaConfig _config;

    public IndexModel(
        ApplicationDbContext context,
        SignInManager<OgmaUser> signInManager,
        ImageUploader uploader,
        OgmaConfig config
    ) {
        _signInManager = signInManager;
        _uploader = uploader;
        _config = config;
        _context = context;
    }
        
    [TempData]
    public string StatusMessage { get; set; }

    [BindProperty] 
    public InputModel Input { get; set; }
        
    public class InputModel
    {
        public string Username { get; init; }
        [DataType(DataType.Upload)]
        public IFormFile Avatar { get; init; }

        public bool DeleteAvatar { get; set; }
        public string Title { get; init; }
        public string Bio { get; init; }
        public string Links { get; set; }
    }

    public class InputModelValidation : AbstractValidator<InputModel>
    {
        public InputModelValidation()
        {
            RuleFor(x => x.Avatar)
                .FileSmallerThan(CTConfig.CFiles.AvatarMaxWeight)
                .FileHasExtension(".jpg", ".jpeg", ".png");
            RuleFor(x => x.Title)
                .MaximumLength(CTConfig.CUser.MaxTitleLength);
            RuleFor(x => x.Bio)
                .MaximumLength(CTConfig.CUser.MaxBioLength);
            RuleFor(x => x.Links)
                .MaximumLines(CTConfig.CUser.MaxLinksAmount);
        }
    }

    private async Task LoadAsync(long? uid)
    {
        Input = await _context.Users
            .Where(u => u.Id == uid)
            .Select(u => new InputModel
            {
                Username = u.UserName,
                Title = u.Title,
                Bio = u.Bio,
                Links = string.Join('\n', u.Links)
            })
            .FirstOrDefaultAsync();
    }

    public async Task<IActionResult> OnGetAsync()
    {
        var uid = User.GetNumericId();
        if (uid is null) return Unauthorized();

        await LoadAsync(uid);
        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        var uid = User.GetNumericId();
        if (uid is null) return Unauthorized();
            
        var user = await _context.Users
            .Where(u => u.Id == uid)
            .FirstOrDefaultAsync();
            
        if (user is null) return NotFound("Unable to load user");
            
        if (!ModelState.IsValid)
        {
            await LoadAsync(uid);
            return Page();
        }

        // If new avatar provided, replace the old one
        if (Input.Avatar is { Length: > 0 })
        {
            // Delete the old avatar if exists
            if (user.AvatarId is not null)
            {
                await _uploader.Delete(user.Avatar, user.AvatarId);
            }

            // Upload the new one
            var file = await _uploader.Upload(
                Input.Avatar,
                "avatars",
                $"U-{user.NormalizedUserName}",
                _config.AvatarWidth,
                _config.AvatarHeight
            );
            user.AvatarId = file.FileId;
            user.Avatar = Path.Join(_config.Cdn, file.Path);
        }
        else if (Input.DeleteAvatar)
        {
            if (user.AvatarId is not null)
            {
                await _uploader.Delete(user.Avatar, user.AvatarId);
            }

            user.AvatarId = null;
            user.Avatar = new Url(_config.AvatarServiceUrl).AppendPathSegment($"{user.UserName}.png").ToString();
            // user.Avatar = Gravatar.Generate(user.Email, new Gravatar.Options
            // {
            //     Default = new Url(_config.AvatarServiceUrl).AppendPathSegment($"{user.UserName}.png").ToString(), 
            //     Rating = Gravatar.Ratings.G
            // });
        }

        if (Input.Title != user.Title)
        {
            user.Title = Input.Title;
        }

        if (Input.Bio != user.Bio)
        {
            user.Bio = Input.Bio;
        }

        user.Links = Input.Links
            .Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            .Where(l => Uri.TryCreate(l, UriKind.RelativeOrAbsolute, out  _))
            .ToList();

        await _context.SaveChangesAsync();

        await _signInManager.RefreshSignInAsync(user);
        StatusMessage = "Your profile has been updated";
        return RedirectToPage();
    }
}