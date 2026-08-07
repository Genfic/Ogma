using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Ogma3.Data;
using Ogma3.Infrastructure.Extensions;
using Ogma3.Services.Mailer;

namespace Ogma3.Areas.Identity.Pages.Account;

public sealed class ResetPinModel(AppDbContext context, IMailer mailer, ILogger<ResetPinModel> logger) : PageModel
{
	public bool Success { get; set; }
	// Handler needed for SafeRouting generator to work

	public IActionResult OnGet() => Page();

	public async Task<IActionResult> OnPostAsync()
	{
		if (User.GetNumericId() is not {} uid || User.GetEmail() is not {} email || User.GetUsername() is not {} name)
		{
			return Page();
		}

		logger.LogInformation("User {Name} ({Id}) requested safety PIN reset", name, uid);

		var raw = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));
		var hash = Convert.ToBase64String(SHA256.HashData(Encoding.UTF8.GetBytes(raw)));

		var rows = await context.Users
			.Where(u => u.Id == uid)
			.ExecuteUpdateAsync(s => s
				.SetProperty(u => u.SafetyPinResetTokenHash, hash)
				.SetProperty(u => u.SafetyPinResetTokenExpiry, DateTimeOffset.UtcNow.AddHours(1)));

		if (rows <= 0)
		{
			return Page();
		}

		var callbackUrl = Url.Page(
			"/Account/ResetPinConfirmation",
			null,
			new { area = "Identity", code = raw },
			Request.Scheme);

		await mailer.SendEmailTemplateAsync(email, "reset-pin", new()
		{
			["name"] = name,
			["link"] = callbackUrl ?? "",
		});

		Success = true;

		return Page();
	}
}