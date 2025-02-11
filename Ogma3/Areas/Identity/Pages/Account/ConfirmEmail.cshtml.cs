#nullable disable

using System.ComponentModel.DataAnnotations;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;
using Ogma3.Data;
using Ogma3.Data.Blacklists;
using Ogma3.Data.CommentsThreads;
using Ogma3.Data.Shelves;

namespace Ogma3.Areas.Identity.Pages.Account;

[AllowAnonymous]
public sealed class ConfirmEmailModel : PageModel
{
	private readonly OgmaUserManager _userManager;
	private readonly ApplicationDbContext _context;

	public ConfirmEmailModel(OgmaUserManager userManager, ApplicationDbContext context)
	{
		_userManager = userManager;
		_context = context;
	}

	// ReSharper disable once MemberCanBePrivate.Global
	[TempData] public required string StatusMessage { get; set; }

	[BindProperty] [Required] public required string UserName { get; set; }

	[BindProperty] [Required] public required string Code { get; set; }

	public IActionResult OnGet(string userName, string code)
	{
		UserName = userName;
		Code = code;

		return Page();
	}

	public async Task<IActionResult> OnPostAsync()
	{
		if (!ModelState.IsValid) return Page();

		var user = await _userManager.FindByNameAsync(UserName);
		if (user is null) return NotFound($"Unable to load user with name '{UserName}'.");

		var code = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(Code));
		var result = await _userManager.ConfirmEmailAsync(user, code);

		StatusMessage = result.Succeeded ? "Thank you for confirming your email." : "Error confirming your email.";

		if (!result.Succeeded) return Page();

		// Setup default blacklists
		var defaultBlockedRatings = await _context.Ratings
			.Where(r => r.BlacklistedByDefault)
			.Select(r => r.Id)
			.ToListAsync();
		var blockedRatings = defaultBlockedRatings.Select(dbr => new BlacklistedRating
		{
			User = user,
			RatingId = dbr,
		});
		_context.BlacklistedRatings.AddRange(blockedRatings);

		// Setup profile comment thread subscription
		var thread = await _context.CommentThreads
			.Where(ct => ct.UserId == user.Id)
			.FirstOrDefaultAsync();

		_context.CommentThreadSubscribers.Add(new CommentThreadSubscriber
		{
			CommentThread = thread,
			OgmaUser = user,
		});

		// Setup default bookshelves
		var shelves = new Shelf[]
		{
			new()
			{
				Name = "Favourites",
				Description = "My favourite stories",
				Color = "#ffff00",
				IsDefault = true,
				IsPublic = true,
				TrackUpdates = true,
				IsQuickAdd = true,
				OwnerId = user.Id,
				IconId = 12,
			},
			new()
			{
				Name = "Read Later",
				Description = "What I plan to read",
				Color = "#5555ff",
				IsDefault = true,
				IsPublic = true,
				TrackUpdates = true,
				IsQuickAdd = true,
				OwnerId = user.Id,
				IconId = 22,
			},
		};
		_context.Shelves.AddRange(shelves);

		await _context.SaveChangesAsync();

		return Page();
	}
}