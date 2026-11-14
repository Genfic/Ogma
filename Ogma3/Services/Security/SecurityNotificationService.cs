using Ogma3.Data;
using Immediate.Injections.Shared;
using Ogma3.Data.Users;
using Ogma3.Services.Mailer;

namespace Ogma3.Services.Security;

/// <summary>
/// Sends security notifications to the user's current database email address after
/// successful credential changes. Delivery failures never roll back committed security state.
/// </summary>
[RegisterScoped]
public sealed class SecurityNotificationService(
	OgmaUserManager userManager,
	IMailer mailer,
	ILogger<SecurityNotificationService> logger)
{
	public async Task SendAsync(OgmaUser user, string subject, string htmlMessage)
	{
		try
		{
			// Trusted contact information comes from the database, not stale email claims
			var email = await userManager.GetEmailAsync(user);
			if (email is null)
			{
				logger.LogWarning("Could not send security notification to user {UserId}: no email address", user.Id);
				return;
			}

			await mailer.SendEmailAsync(email, subject, htmlMessage);
		}
		catch (Exception e)
		{
			logger.LogError(e, "Failed to send security notification to user {UserId}", user.Id);
		}
	}
}
