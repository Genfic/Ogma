using System.Buffers.Text;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using Immediate.Injections.Shared;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Ogma3.Data;
using Ogma3.Data.SecurityTransactions;
using Ogma3.Data.Users;
using Ogma3.Infrastructure.Extensions;

namespace Ogma3.Services.Security;

/// <summary>
/// Shared mechanism for authorizing account-security changes: creating pending security
/// transactions, verifying fresh account proof against them, and consuming the resulting
/// grants at the protected write.
///
/// State is persisted in PostgreSQL (never process memory), bound to the current user and
/// application session, and consumed atomically with a single conditional UPDATE.
/// </summary>
[RegisterScoped]
public sealed class SecurityAuthorizationService(
	AppDbContext context,
	ISecuritySessionProvider sessionProvider,
	SignInManager<OgmaUser> signInManager,
	OgmaUserManager userManager,
	TimeProvider timeProvider,
	ILogger<SecurityAuthorizationService> logger)
{
	/// <summary>How long a challenge may be completed after creation.</summary>
	public static readonly TimeSpan TransactionLifetime = TimeSpan.FromMinutes(10);

	/// <summary>How long completed proof remains usable at the protected write.</summary>
	public static readonly TimeSpan GrantValidity = TimeSpan.FromMinutes(5);

	// ======================================================================
	// Creation
	// ======================================================================

	/// <summary>
	/// Creates a new pending security transaction. Any previous unverified transactions for
	/// the user are superseded. Does not reset any attempt budget: failed-attempt counting
	/// lives in Identity's access-failure store, not here.
	/// </summary>
	public async Task<SecurityTransaction> BeginAsync(
		OgmaUser user, SecurityOperation operation, string? target = null, string? proposedSecret = null)
	{
		var sessionId = await sessionProvider.GetSessionIdAsync()
		                ?? throw new InvalidOperationException("Cannot begin a security transaction without an authenticated application session.");

		// Supersede previous unverified challenges
		await context.SecurityTransactions
			.Where(t => t.UserId == user.Id && t.VerifiedAt == null && t.ConsumedAt == null)
			.ExecuteDeleteAsync();

		var now = timeProvider.GetUtcNow();
		var txn = new SecurityTransaction
		{
			Id = NewTransactionId(),
			UserId = user.Id,
			SessionId = sessionId,
			Operation = operation,
			Target = target,
			SecurityStamp = user.SecurityStamp,
			ProposedSecret = proposedSecret,
			AllowedProofs = await DetermineAllowedProofsAsync(user, operation),
			CreatedAt = now,
			ExpiresAt = now + TransactionLifetime,
		};

		context.Add(txn);
		await context.SaveChangesAsync();
		return txn;
	}

	/// <summary>
	/// Reuses an outstanding (unconsumed) TOTP setup transaction for the current session —
	/// preserving its staged proposed secret and any verification already completed — or
	/// stages a new one with a freshly generated authenticator secret.
	/// </summary>
	public async Task<SecurityTransaction> GetOrCreateTotpSetupAsync(OgmaUser user)
	{
		var sessionId = await sessionProvider.GetSessionIdAsync()
		                ?? throw new InvalidOperationException("No authenticated application session.");

		var txn = await context.SecurityTransactions
			.Where(t => t.UserId == user.Id
				&& t.SessionId == sessionId
				&& t.Operation == SecurityOperation.SetupTotp
				&& t.Target == null
				&& t.ConsumedAt == null
				&& t.ExpiresAt > timeProvider.GetUtcNow())
			.FirstOrDefaultAsync();

		if (txn is not null) return txn;

		return await BeginAsync(user, SecurityOperation.SetupTotp, proposedSecret: TotpStaging.GenerateSecret());
	}

	/// <summary>
	/// Proofs the account may currently use for the given operation, resolved from
	/// authoritative account state — only methods the account can actually use.
	/// External provider re-verification is only accepted when neither a password nor a
	/// passkey exists, so a working password can never be replaced by a provider shortcut.
	/// </summary>
	public async Task<SecurityProof> DetermineAllowedProofsAsync(OgmaUser user, SecurityOperation operation)
	{
		var passkeys = await userManager.GetPasskeysAsync(user);
		var externalLogins = await userManager.GetLoginsAsync(user);
		var hasPassword = user.PasswordHash is not null;

		var proofs = SecurityProof.None;
		if (passkeys.Count > 0) proofs |= SecurityProof.Passkey;
		if (hasPassword) proofs |= SecurityProof.Password;
		if (passkeys.Count == 0 && !hasPassword && externalLogins.Count > 0) proofs |= SecurityProof.ExternalLogin;

		// Password disabling and re-adding always require a passkey
		if (operation is SecurityOperation.RemovePassword or SecurityOperation.AddPassword)
		{
			return passkeys.Count > 0 ? SecurityProof.Passkey : proofs & SecurityProof.ExternalLogin;
		}

		return proofs;
	}

	// ======================================================================
	// Verification (the reauthentication page)
	// ======================================================================

	/// <summary>
	/// Loads and validates a pending transaction for the reauthentication page.
	/// </summary>
	public async Task<(SecurityTransaction? Transaction, SecurityTransactionRejection Rejection)> GetPendingAsync(
		string transactionId, OgmaUser user)
	{
		var sessionId = await sessionProvider.GetSessionIdAsync();
		if (sessionId is null) return (null, SecurityTransactionRejection.SessionMismatch);

		var txn = await context.SecurityTransactions.FirstOrDefaultAsync(t => t.Id == transactionId);
		var rejection = SecurityTransactionValidator.ValidatePending(
			txn, user.Id, sessionId, txn?.Operation ?? default, txn?.Target, user.SecurityStamp, timeProvider.GetUtcNow());

		return (txn, rejection);
	}

	/// <summary>Result of a proof verification attempt. Never reveals which factor failed.</summary>
	public readonly record struct VerificationResult(bool Success);

	private async Task<(SecurityTransaction? Transaction, string? SessionId)> ResolvePendingAsync(string transactionId)
	{
		var sessionId = await sessionProvider.GetSessionIdAsync();
		if (sessionId is null) return (null, null);

		var txn = await context.SecurityTransactions.FirstOrDefaultAsync(t => t.Id == transactionId);
		if (txn is null) return (null, sessionId);

		var user = await context.Users.FirstOrDefaultAsync(u => u.Id == txn.UserId);
		if (user is null) return (null, sessionId);

		var rejection = SecurityTransactionValidator.ValidatePending(
			txn, user.Id, sessionId, txn.Operation, txn.Target, user.SecurityStamp, timeProvider.GetUtcNow());

		return rejection is SecurityTransactionRejection.None ? (txn, sessionId) : (null, sessionId);
	}

	public async Task<VerificationResult> VerifyPasswordAsync(string transactionId, string password, string? totpCode)
	{
		var (txn, _) = await ResolvePendingAsync(transactionId);
		if (txn is null)
		{
			return GenericFailure();
		}

		var user = await context.Users.FirstAsync(u => u.Id == txn.UserId);

		if ((txn.AllowedProofs & SecurityProof.Password) == 0 || user.PasswordHash is null)
		{
			return GenericFailure();
		}

		// Respect lockout — same policy as ordinary sign-in
		if (await userManager.IsLockedOutAsync(user))
		{
			logger.LogWarning("Reauthentication attempt for locked-out user {UserId}", user.Id);
			return GenericFailure();
		}

		if (!await userManager.CheckPasswordAsync(user, password))
		{
			await userManager.AccessFailedAsync(user);
			logger.LogWarning("Reauthentication password verification failed for user {UserId}", user.Id);
			return GenericFailure();
		}

		// A correct password must not reset failed-attempt accounting before the second factor succeeds
		if (user.TwoFactorEnabled)
		{
			if (string.IsNullOrWhiteSpace(totpCode))
			{
				await userManager.AccessFailedAsync(user);
				logger.LogWarning("Reauthentication TOTP verification failed for user {UserId} (missing code)", user.Id);
				return GenericFailure();
			}

			var totpValid = await userManager.VerifyTwoFactorTokenAsync(
				user, userManager.Options.Tokens.AuthenticatorTokenProvider, totpCode);
			if (!totpValid)
			{
				await userManager.AccessFailedAsync(user);
				logger.LogWarning("Reauthentication TOTP verification failed for user {UserId}", user.Id);
				return GenericFailure();
			}
		}

		// Remembered-browser MFA state is never consulted here — a fresh code is always required

		await userManager.ResetAccessFailedCountAsync(user);
		return await MarkVerifiedAsync(txn, SecurityProof.Password | (user.TwoFactorEnabled ? SecurityProof.Totp : SecurityProof.None));
	}

	public async Task<VerificationResult> VerifyPasskeyAsync(string transactionId, string credentialJson)
	{
		var (txn, _) = await ResolvePendingAsync(transactionId);
		if (txn is null)
		{
			return GenericFailure();
		}

		var user = await context.Users.FirstAsync(u => u.Id == txn.UserId);

		if ((txn.AllowedProofs & SecurityProof.Passkey) == 0)
		{
			return GenericFailure();
		}

		// Identity's assertion-verification primitives — not the ordinary passkey sign-in endpoint
		var assertionResult = await signInManager.PerformPasskeyAssertionAsync(credentialJson);
		if (!assertionResult.Succeeded)
		{
			logger.LogWarning("Reauthentication passkey assertion failed for user {UserId}", user.Id);
			return GenericFailure();
		}

		// The assertion must belong to the current account
		if (assertionResult.User.Id != user.Id)
		{
			logger.LogWarning("Passkey assertion during reauthentication belonged to a different user than the pending transaction");
			return GenericFailure();
		}

		// Server-side user verification requirement — client options alone are insufficient
		if (!PasskeyUserVerification.IsUserVerified(credentialJson))
		{
			logger.LogWarning("Passkey assertion during reauthentication lacked user verification for user {UserId}", user.Id);
			return GenericFailure();
		}

		// Preserve Identity's passkey counter/metadata update behavior
		var updateResult = await userManager.AddOrUpdatePasskeyAsync(user, assertionResult.Passkey);
		if (!updateResult.Succeeded)
		{
			logger.LogWarning("Reauthentication passkey metadata update failed for user {UserId}", user.Id);
			return GenericFailure();
		}

		return await MarkVerifiedAsync(txn, SecurityProof.Passkey);
	}

	/// <summary>
	/// Verifies an external provider re-assertion on the reauthentication callback:
	/// the provider identity returned by the challenge must be one already linked to the
	/// current account. Never creates a new application session.
	/// </summary>
	public async Task<VerificationResult> VerifyExternalLoginAsync(string transactionId)
	{
		var (txn, _) = await ResolvePendingAsync(transactionId);
		if (txn is null)
		{
			return GenericFailure();
		}

		var user = await context.Users.FirstAsync(u => u.Id == txn.UserId);

		if ((txn.AllowedProofs & SecurityProof.ExternalLogin) == 0)
		{
			return GenericFailure();
		}

		var info = await signInManager.GetExternalLoginInfoAsync();
		if (info?.ProviderKey is null)
		{
			logger.LogWarning("Reauthentication external verification returned no login info for user {UserId}", user.Id);
			return GenericFailure();
		}

		var logins = await userManager.GetLoginsAsync(user);
		if (!logins.Any(l => l.LoginProvider == info.LoginProvider && l.ProviderKey == info.ProviderKey))
		{
			logger.LogWarning("External provider identity presented during reauthentication is not linked to user {UserId}", user.Id);
			return GenericFailure();
		}

		// The external-scheme cookie has served its purpose; never leave it behind
		await signInManager.Context.SignOutAsync(IdentityConstants.ExternalScheme);

		return await MarkVerifiedAsync(txn, SecurityProof.ExternalLogin);
	}

	/// <summary>
	/// Assertion request options for reauthentication, bound to the current user and always
	/// requiring user verification. The framework assertion state cookie is written by
	/// <see cref="SignInManager{OgmaUser}.MakePasskeyRequestOptionsAsync"/>.
	/// </summary>
	public async Task<string> MakePasskeyOptionsAsync(OgmaUser user)
	{
		var optionsJson = await signInManager.MakePasskeyRequestOptionsAsync(user);

		// Force user verification in the options presented to the client; the server-side
		// UV-bit check in VerifyPasskeyAsync remains the authoritative enforcement.
		try
		{
			using var doc = JsonDocument.Parse(optionsJson);
			using var buffer = new MemoryStream();
			using (var writer = new Utf8JsonWriter(buffer))
			{
				writer.WriteStartObject();
				foreach (var property in doc.RootElement.EnumerateObject())
				{
					if (property.NameEquals("userVerification"))
					{
						writer.WriteString("userVerification", "required");
					}
					else
					{
						property.WriteTo(writer);
					}
				}

				writer.WriteEndObject();
			}

			return Encoding.UTF8.GetString(buffer.ToArray());
		}
		catch (JsonException)
		{
			return optionsJson;
		}
	}

	private async Task<VerificationResult> MarkVerifiedAsync(SecurityTransaction txn, SecurityProof completedProof)
	{
		var now = timeProvider.GetUtcNow();
		txn.CompletedProof = completedProof;
		txn.VerifiedAt = now;
		await context.SaveChangesAsync();

		logger.LogInformation("Security transaction {TransactionId} (user {UserId}, operation {Operation}) verified",
			txn.Id, txn.UserId, txn.Operation);

		return new(true);
	}

	// ======================================================================
	// Consumption (the protected write)
	// ======================================================================

	/// <summary>
	/// True when a valid, unconsumed, unexpired grant exists for this user, session,
	/// operation and target. Non-mutating — for deciding whether to redirect to reauthentication.
	/// </summary>
	public async Task<bool> HasVerifiedGrantAsync(OgmaUser user, SecurityOperation operation, string? target = null)
	{
		var sessionId = await sessionProvider.GetSessionIdAsync();
		if (sessionId is null) return false;

		var txn = await FindGrantAsync(user.Id, sessionId, operation, target);
		return SecurityTransactionValidator.ValidateGrant(
			       txn, user.Id, sessionId, operation, target, user.SecurityStamp, timeProvider.GetUtcNow(), GrantValidity)
			== SecurityTransactionRejection.None;
	}

	/// <summary>
	/// Atomically consumes a verified grant. All bindings (user, session, operation, target,
	/// security stamp, validity) are validated, then a single conditional UPDATE guarantees
	/// the grant cannot be consumed twice.
	/// </summary>
	public async Task<bool> ConsumeAsync(OgmaUser user, SecurityOperation operation, string? target = null)
	{
		var sessionId = await sessionProvider.GetSessionIdAsync();
		if (sessionId is null)
		{
			logger.LogWarning("Grant consumption attempted without an application session (user {UserId})", user.Id);
			return false;
		}

		var txn = await FindGrantAsync(user.Id, sessionId, operation, target);
		var now = timeProvider.GetUtcNow();
		var rejection = SecurityTransactionValidator.ValidateGrant(
			txn, user.Id, sessionId, operation, target, user.SecurityStamp, now, GrantValidity);

		if (rejection is not SecurityTransactionRejection.None)
		{
			logger.LogInformation("Grant for operation {Operation} (user {UserId}) rejected: {Rejection}", operation, user.Id, rejection);
			return false;
		}

		var rows = await context.SecurityTransactions
			.Where(t => t.Id == txn!.Id && t.ConsumedAt == null)
			.ExecuteUpdateAsync(s => s.SetProperty(t => t.ConsumedAt, now));

		if (rows == 1)
		{
			logger.LogInformation("Security transaction {TransactionId} consumed (user {UserId}, operation {Operation})",
				txn!.Id, user.Id, operation);
		}

		return rows == 1;
	}

	private async Task<SecurityTransaction?> FindGrantAsync(
		long userId, string sessionId, SecurityOperation operation, string? target)
	{
		IQueryable<SecurityTransaction> query = context.SecurityTransactions
			.Where(t => t.UserId == userId && t.SessionId == sessionId && t.Operation == operation)
			.Where(t => t.VerifiedAt != null && t.ConsumedAt == null)
			.Where(t => t.Target == target);

		return await query.OrderByDescending(t => t.VerifiedAt).FirstOrDefaultAsync();
	}

	// ======================================================================
	// Invalidation and cleanup
	// ======================================================================

	/// <summary>
	/// Deletes every outstanding (pending or verified, unconsumed) transaction for a user.
	/// Called whenever credentials change.
	/// </summary>
	public async Task<int> InvalidateForUserAsync(long userId)
		=> await context.SecurityTransactions
			.Where(t => t.UserId == userId && t.ConsumedAt == null)
			.ExecuteDeleteAsync();

	/// <summary>
	/// Deletes expired and old consumed transactions. Run by a recurring job.
	/// </summary>
	public async Task<int> SweepAsync(CancellationToken ct = default)
	{
		var now = timeProvider.GetUtcNow();
		var consumed = await context.SecurityTransactions
			.Where(t => t.ConsumedAt != null && t.ConsumedAt < now.AddDays(-1))
			.ExecuteDeleteAsync(ct);

		var expired = await context.SecurityTransactions
			.Where(t => t.ConsumedAt == null && t.ExpiresAt < now)
			.ExecuteDeleteAsync(ct);

		return consumed + expired;
	}

	// ======================================================================
	// Transactional mutation helpers
	// ======================================================================

	/// <summary>
	/// Begins an explicit database transaction and takes a row lock on the user,
	/// serializing per-user credential mutations so that concurrent removals cannot
	/// eliminate the last usable login path.
	/// </summary>
	public async Task<IDbContextTransaction> BeginCredentialMutationAsync(long userId)
	{
		var dbTx = await context.Database.BeginTransactionAsync();

		// Row-level lock; the lock is held for the duration of the transaction
		await context.Database.ExecuteSqlRawAsync(
			"SELECT \"Id\" FROM \"AspNetUsers\" WHERE \"Id\" = {0}", userId);

		return dbTx;
	}

	private VerificationResult GenericFailure()
	{
		// Generic verification error — no detail about which factor failed
		return new(false);
	}

	private static string NewTransactionId()
		=> Base64Url.EncodeToString(RandomNumberGenerator.GetBytes(32));
}
