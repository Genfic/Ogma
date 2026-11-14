using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Ogma3.Data;
using Ogma3.Data.SecurityTransactions;
using Ogma3.Data.Users;
using Ogma3.Infrastructure.Extensions;
using Ogma3.Services.Security;

namespace Ogma3.Areas.Identity.Pages.Account.Manage;

public sealed class DownloadPersonalDataModel(
	AppDbContext context,
	UserManager<OgmaUser> userManager,
	SecurityAuthorizationService securityAuth,
	ILogger<DownloadPersonalDataModel> logger)
	: PageModel
{
	public async Task<IActionResult> OnPostAsync()
	{
		if (User.GetNumericId() is not { } uid) return Unauthorized();

		var identityUser = await userManager.GetUserAsync(User);
		if (identityUser is null)
		{
			return NotFound($"Unable to load user with ID {uid}.");
		}

		// Exporting personal data requires fresh account proof
		if (!await securityAuth.HasVerifiedGrantAsync(identityUser, SecurityOperation.ExportPersonalData))
		{
			var txn = await securityAuth.BeginAsync(identityUser, SecurityOperation.ExportPersonalData);
			return ReauthRequired.RedirectToReauth(txn, Request.Path.ToString(), this);
		}

		if (!await securityAuth.ConsumeAsync(identityUser, SecurityOperation.ExportPersonalData))
		{
			var txn = await securityAuth.BeginAsync(identityUser, SecurityOperation.ExportPersonalData);
			return ReauthRequired.RedirectToReauth(txn, Request.Path.ToString(), this);
		}

		var user = await context.Users
			.Where(u => u.Id == uid)
			.ProjectToDto()
			.FirstOrDefaultAsync();

		if (user is null)
		{
			return NotFound($"Unable to load user with ID {uid}.");
		}

		logger.LogInformation("User with ID {UserId} asked for their personal data", uid);

		var json = JsonSerializer.Serialize(user, UserPersonalDataJsonContext.Default.UserPersonalData);
		
		Response.Headers.ContentDisposition = "attachment; filename=PersonalData.json";
		return new FileContentResult(Encoding.UTF8.GetBytes(json), "text/json");
	}
}

[JsonSerializable(typeof(UserPersonalData))]
[JsonSourceGenerationOptions(WriteIndented = true)]
public sealed partial class UserPersonalDataJsonContext : JsonSerializerContext;