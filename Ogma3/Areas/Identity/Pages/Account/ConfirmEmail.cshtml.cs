using System.Text;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;
using Ogma3.Data;
using Ogma3.Data.Blacklists;
using Ogma3.Data.CommentsThreads;
using Ogma3.Data.Images;
using Ogma3.Data.Shelves;
using Ogma3.Services.GeneratedImagesService;

namespace Ogma3.Areas.Identity.Pages.Account;

[AllowAnonymous]
public sealed class ConfirmEmailModel(
	OgmaUserManager userManager,
	ApplicationDbContext context,
	GeneratedImagesService imagesService,
	ILogger<ConfirmEmailModel> logger
) : PageModel
{
	[TempData] public required string StatusMessage { get; set; }

	public async Task<IActionResult> OnGetAsync(string? userName = null, string? code = null)
	{
		if (userName is null || code is null)
		{
			StatusMessage = "Error: Invalid confirmation link.";
			return Page();
		}

		var user = await userManager.FindByNameAsync(userName);
		if (user is null)
		{
			logger.LogWarning("Attempt to confirm email for nonexistent user '{UserName}'.", userName);
			StatusMessage = "Error: Invalid confirmation link.";
			return Page();
		}

		var decodedCode = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(code));
		var result = await userManager.ConfirmEmailAsync(user, decodedCode);

		StatusMessage = result.Succeeded ? "Thank you for confirming your email." : "Error confirming your email.";

		if (!result.Succeeded)
		{
			return Page();
		}

		// Setup default avatar
		var avatar = new Image
		{
			Url = imagesService.GenerateAvatarUrl(user.UserName),
		};
		context.Images.Add(avatar);
		user.Avatar = avatar;


		// Setup default blacklists
		var defaultBlockedRatings = await context.Ratings
			.Where(r => r.BlacklistedByDefault)
			.Select(r => r.Id)
			.ToListAsync();
		var blockedRatings = defaultBlockedRatings.Select(dbr => new BlacklistedRating
		{
			User = user,
			RatingId = dbr,
		});
		context.BlacklistedRatings.AddRange(blockedRatings);

		// Setup profile comment thread subscription
		var thread = await context.CommentThreads
			.Where(ct => ct.UserId == user.Id)
			.FirstOrDefaultAsync();

		context.CommentThreadSubscribers.Add(new CommentThreadSubscriber
		{
			CommentThread = thread ?? new(),
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
				IconId = 11,
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
		context.Shelves.AddRange(shelves);

		await context.SaveChangesAsync();

		StatusMessage = "Thank you for confirming your email.";

		return Routes.Areas.Identity.Pages.Account_Login.Get().Redirect(this);
	}
}