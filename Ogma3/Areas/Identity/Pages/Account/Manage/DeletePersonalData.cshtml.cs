#nullable disable

using System.ComponentModel.DataAnnotations;
using HashidsNet;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Ogma3.Data;
using Ogma3.Data.Users;
using Index = Routes.Pages.Index;

namespace Ogma3.Areas.Identity.Pages.Account.Manage;

public sealed class DeletePersonalDataModel : PageModel
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

	[BindProperty] public required InputModel Input { get; set; }

	public sealed class InputModel
	{
		[Required]
		[DataType(DataType.Password)]
		public required string Password { get; set; }
	}

	public required bool RequirePassword { get; set; }

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

		var hashids = new Hashids(minHashLength: 6);

		// Clean basic data
		user.UserName = $"Deleted User {hashids.EncodeLong(user.Id)}";
		user.NormalizedUserName = user.UserName.ToUpperInvariant().Normalize();
		user.Email = "noreply@example.com";
		user.NormalizedEmail = user.Email.ToUpperInvariant().Normalize();
		user.Bio = null;
		user.Title = null;
		user.DeletedAt = DateTimeOffset.UtcNow;
		user.LastActive = DateTimeOffset.UtcNow;
		user.RegistrationDate = DateTimeOffset.UtcNow;

		// Delete related data
		await _context.BlockedUsers.Where(ub => ub.BlockedUserId == user.Id || ub.BlockingUserId == user.Id).ExecuteDeleteAsync();
		await _context.BlacklistedRatings.Where(br => br.UserId == user.Id).ExecuteDeleteAsync();
		await _context.BlacklistedTags.Where(bt => bt.UserId == user.Id).ExecuteDeleteAsync();
		await _context.FollowedUsers.Where(uf => uf.FollowedUserId == user.Id || uf.FollowingUserId == user.Id).ExecuteDeleteAsync();
		await _context.CommentThreadSubscribers.Where(cts => cts.OgmaUserId == user.Id).ExecuteDeleteAsync();
		await _context.NotificationRecipients.Where(nr => nr.RecipientId == user.Id).ExecuteDeleteAsync();
		await _context.UserLogins.Where(ul => ul.UserId == user.Id).ExecuteDeleteAsync();


		var result = await _userManager.UpdateAsync(user);
		if (!result.Succeeded)
		{
			_logger.LogError("Couldn't delete information of user {Id} because of {@Errors}", user.Id, result.Errors);
			throw new InvalidOperationException("Unexpected error occurred deleting user data.");
		}

		await _signInManager.SignOutAsync();

		_logger.LogInformation("User with ID '{UserId}' deleted themselves", user.Id);

		return Index.Get().Redirect(this);
	}
}