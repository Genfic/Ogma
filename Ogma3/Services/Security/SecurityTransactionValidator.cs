using Ogma3.Data.SecurityTransactions;

namespace Ogma3.Services.Security;

public enum SecurityTransactionRejection
{
	None,
	NotFound,
	UserMismatch,
	SessionMismatch,
	OperationMismatch,
	TargetMismatch,
	StampStale,
	Expired,
	AlreadyConsumed,
	NotVerified,
	ProofUsedTooLate,
	ProofNotPermitted,
}

/// <summary>
/// Pure validation of a pending security transaction against the authoritative current state.
/// Called both when showing the reauthentication page and when consuming the grant at the
/// protected write — the latter is the authoritative check.
/// </summary>
public static class SecurityTransactionValidator
{
	/// <summary>
	/// Validates a *pending* transaction (before proof is presented). Does not care about
	/// verification state — callers inspect <see cref="SecurityTransaction.VerifiedAt"/> to decide
	/// whether proof is still needed.
	/// </summary>
	public static SecurityTransactionRejection ValidatePending(
		SecurityTransaction? transaction,
		long currentUserId,
		string currentSessionId,
		SecurityOperation operation,
		string? target,
		string currentSecurityStamp,
		DateTimeOffset now)
	{
		if (transaction is null) return SecurityTransactionRejection.NotFound;
		if (transaction.UserId != currentUserId) return SecurityTransactionRejection.UserMismatch;
		if (transaction.SessionId != currentSessionId) return SecurityTransactionRejection.SessionMismatch;
		if (transaction.Operation != operation) return SecurityTransactionRejection.OperationMismatch;
		if (transaction.Target != target) return SecurityTransactionRejection.TargetMismatch;
		if (transaction.SecurityStamp != currentSecurityStamp) return SecurityTransactionRejection.StampStale;
		if (now >= transaction.ExpiresAt) return SecurityTransactionRejection.Expired;
		if (transaction.ConsumedAt is not null) return SecurityTransactionRejection.AlreadyConsumed;

		return SecurityTransactionRejection.None;
	}

	/// <summary>
	/// Validates a *verified but not yet consumed* grant at the protected write.
	/// The grant is usable for <see cref="GrantValidity"/> after <see cref="SecurityTransaction.VerifiedAt"/>.
	/// </summary>
	public static SecurityTransactionRejection ValidateGrant(
		SecurityTransaction? transaction,
		long currentUserId,
		string currentSessionId,
		SecurityOperation operation,
		string? target,
		string currentSecurityStamp,
		DateTimeOffset now,
		TimeSpan grantValidity)
	{
		if (transaction is null) return SecurityTransactionRejection.NotFound;
		if (transaction.UserId != currentUserId) return SecurityTransactionRejection.UserMismatch;
		if (transaction.SessionId != currentSessionId) return SecurityTransactionRejection.SessionMismatch;
		if (transaction.Operation != operation) return SecurityTransactionRejection.OperationMismatch;
		if (transaction.Target != target) return SecurityTransactionRejection.TargetMismatch;
		if (transaction.SecurityStamp != currentSecurityStamp) return SecurityTransactionRejection.StampStale;
		if (now >= transaction.ExpiresAt) return SecurityTransactionRejection.Expired;
		if (transaction.VerifiedAt is null) return SecurityTransactionRejection.NotVerified;
		if (transaction.ConsumedAt is not null) return SecurityTransactionRejection.AlreadyConsumed;
		if (transaction.VerifiedAt.Value + grantValidity < now) return SecurityTransactionRejection.ProofUsedTooLate;
		if (transaction.CompletedProof is not { } proof) return SecurityTransactionRejection.NotVerified;
		if ((proof & transaction.AllowedProofs) == SecurityProof.None) return SecurityTransactionRejection.ProofNotPermitted;

		return SecurityTransactionRejection.None;
	}
}
