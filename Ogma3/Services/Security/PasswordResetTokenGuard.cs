using Immediate.Injections.Shared;
using System.Security.Cryptography;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Ogma3.Data;
using Ogma3.Data.Users;

namespace Ogma3.Services.Security;

/// <summary>
/// Guards the ordinary forgot-password/reset-password flow for passwordless accounts and
/// races with password removal:
///
/// - A passwordless account never regains password authentication through reset endpoints.
/// - Issued reset tokens are tracked by hash so only the newest outstanding token is
///   consumable, and removing the password clears the hash, rejecting tokens issued earlier.
/// </summary>
[RegisterScoped]
public sealed class PasswordResetTokenGuard(AppDbContext context, TimeProvider timeProvider)
{
	/// <summary>Lifetime of the outstanding reset token marker; matches the Identity token lifetime.</summary>
	public static readonly TimeSpan TokenLifetime = TimeSpan.FromDays(1);

	/// <summary>
	/// Records the newly issued reset token as the only outstanding one for the account.
	/// </summary>
	public async Task IssueAsync(OgmaUser user, string token)
	{
		var now = timeProvider.GetUtcNow();
		user.PasswordResetTokenHash = HashToken(token);
		user.PasswordResetTokenExpiry = now + TokenLifetime;
		await context.SaveChangesAsync();
	}

	/// <summary>
	/// Validates an outstanding reset token without mutating anything (for confirmation GETs).
	/// </summary>
	public async Task<bool> IsOutstandingAsync(long userId, string token)
	{
		var hash = HashToken(token);
		var now = timeProvider.GetUtcNow();
		return await context.Users
			.Where(u => u.Id == userId)
			.Where(u => u.PasswordResetTokenHash == hash && u.PasswordResetTokenExpiry > now)
			.AnyAsync();
	}

	/// <summary>
	/// Atomically consumes the outstanding reset token. Rejects consumed, superseded,
	/// expired tokens and any token issued before a password removal (which clears the hash).
	/// </summary>
	public async Task<bool> ConsumeAsync(long userId, string token)
	{
		var hash = HashToken(token);
		var now = timeProvider.GetUtcNow();
		var rows = await context.Users
			.Where(u => u.Id == userId)
			.Where(u => u.PasswordResetTokenHash == hash && u.PasswordResetTokenExpiry > now)
			.ExecuteUpdateAsync(s => s
				.SetProperty(u => u.PasswordResetTokenHash, (string?)null)
				.SetProperty(u => u.PasswordResetTokenExpiry, (DateTimeOffset?)null));

		return rows == 1;
	}

	/// <summary>
	/// Clears the outstanding reset token marker — used when the password is removed.
	/// </summary>
	public async Task ClearAsync(long userId)
	{
		await context.Users
			.Where(u => u.Id == userId)
			.Where(u => u.PasswordResetTokenHash != null)
			.ExecuteUpdateAsync(s => s
				.SetProperty(u => u.PasswordResetTokenHash, (string?)null)
				.SetProperty(u => u.PasswordResetTokenExpiry, (DateTimeOffset?)null));
	}

	/// <summary>SHA-256 of the raw token; tokens are never stored in the clear.</summary>
	public static string HashToken(string token)
		=> Convert.ToBase64String(SHA256.HashData(Encoding.UTF8.GetBytes(token)));
}
