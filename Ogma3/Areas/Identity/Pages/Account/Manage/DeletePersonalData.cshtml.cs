using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using HashidsNet;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using Ogma3.Data;
using Ogma3.Data.Blacklists;
using Ogma3.Data.CommentsThreads;
using Ogma3.Data.Notifications;
using Ogma3.Data.Users;

namespace Ogma3.Areas.Identity.Pages.Account.Manage;

public class DeletePersonalDataModel : PageModel
{
    private readonly UserManager<OgmaUser> _userManager;
    private readonly SignInManager<OgmaUser> _signInManager;
    private readonly ApplicationDbContext _context;
    private readonly ILogger<DeletePersonalDataModel> _logger;

    public DeletePersonalDataModel(
        UserManager<OgmaUser> userManager,
        SignInManager<OgmaUser> signInManager,
        ILogger<DeletePersonalDataModel> logger, 
        ApplicationDbContext context)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _logger = logger;
        _context = context;
    }

    [BindProperty]
    public InputModel Input { get; set; }

    public class InputModel
    {
        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }

    public bool RequirePassword { get; set; }

    public async Task<IActionResult> OnGet()
    {
        var user = await _userManager.GetUserAsync(User);
        if (user is null)
        {
            return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
        }

        RequirePassword = await _userManager.HasPasswordAsync(user);
        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        var user = await _userManager.GetUserAsync(User);
        if (user is null)
        {
            return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
        }

        RequirePassword = await _userManager.HasPasswordAsync(user);
        if (RequirePassword)
        {
            if (!await _userManager.CheckPasswordAsync(user, Input.Password))
            {
                ModelState.AddModelError(string.Empty, "Incorrect password.");
                return Page();
            }
        }

        var hashids = new Hashids();

        // Clean basic data
        user.UserName = $"Deleted User #{hashids.EncodeLong(user.Id)}";
        user.NormalizedUserName = user.UserName.ToUpperInvariant().Normalize();
        user.Email = string.Empty;
        user.NormalizedEmail = string.Empty;
        user.Bio = null;
        user.Title = null;
        user.DeletedAt = DateTime.Now;
        user.LastActive = DateTime.Now;
        user.RegistrationDate = DateTime.Now;
        
        // Delete related data
        await _context.DeleteRangeAsync<UserBlock>(ub => ub.BlockedUserId == user.Id || ub.BlockingUserId == user.Id);
        await _context.DeleteRangeAsync<BlacklistedRating>(br => br.UserId == user.Id);
        await _context.DeleteRangeAsync<BlacklistedTag>(bt => bt.UserId == user.Id);
        await _context.DeleteRangeAsync<UserFollow>(uf => uf.FollowedUserId == user.Id || uf.FollowingUserId == user.Id);
        await _context.DeleteRangeAsync<CommentsThreadSubscriber>(cts => cts.OgmaUserId == user.Id);
        await _context.DeleteRangeAsync<NotificationRecipients>(nr => nr.RecipientId == user.Id);
        
        
        var result = await _userManager.UpdateAsync(user);
        if (!result.Succeeded)
        {
            _logger.LogError("Couldn't delete information of user {Id} because of {@Errors}", user.Id, result.Errors);
            throw new InvalidOperationException("Unexpected error occurred deleting user data.");
        }
        
        await _signInManager.SignOutAsync();

        _logger.LogInformation("User with ID '{UserId}' deleted themselves", user.Id);

        return Redirect("~/");
    }
}