namespace Ogma3.Data.SecurityTransactions;

[AutoDbSet]
public sealed class SecurityTransaction
{
	/// <summary>
	/// Cryptographically unpredictable identifier, safe to use in URLs.
	/// </summary>
	public required string Id { get; init; }

	public required long UserId { get; init; }

	/// <summary>
	/// Identifier of the application session (cookie) that created the transaction.
	/// Transactions are never usable from a different session.
	/// </summary>
	public required string SessionId { get; init; }

	public required SecurityOperation Operation { get; init; }

	/// <summary>
	/// Consequential target the transaction is bound to, e.g. a passkey credential ID,
	/// an external login provider, or a proposed new email. `null` when the operation has no known target.
	/// </summary>
	public string? Target { get; init; }

	/// <summary>
	/// Authoritative user security stamp at creation time; transactions are stale when it changes.
	/// </summary>
	public required string SecurityStamp { get; init; }

	/// <summary>
	/// Proposed value staged for multi-stage operations, e.g. a replacement TOTP secret.
	/// </summary>
	public string? ProposedSecret { get; set; }

	/// <summary>
	/// Proofs the current account may use to verify. Bit flags.
	/// </summary>
	public required SecurityProof AllowedProofs { get; init; }

	/// <summary>
	/// Proofs that were actually presented, when verification succeeded.
	/// </summary>
	public SecurityProof? CompletedProof { get; set; }

	public required DateTimeOffset CreatedAt { get; init; }

	public required DateTimeOffset ExpiresAt { get; init; }

	public DateTimeOffset? VerifiedAt { get; set; }

	public DateTimeOffset? ConsumedAt { get; set; }
}

/// <summary>
/// Security state changes that must be authorized through reauthentication.
/// </summary>
[EnumExtensions]
public enum SecurityOperation
{
	AddPasskey,
	RemovePasskey,
	AddPassword,
	ChangePassword,
	RemovePassword,
	SetupTotp,
	DisableTotp,
	ResetTotp,
	GenerateRecoveryCodes,
	ChangeEmail,
	RemoveExternalLogin,
	LinkExternalLogin,
	CreateSafetyPin,
	ChangeSafetyPin,
	DeleteAccount,
	ExportPersonalData,
}

/// <summary>
/// Proof methods that can authorize a security transaction.
/// <see cref="Totp"/> is never sufficient on its own — it only strengthens a password proof.
/// </summary>
[EnumExtensions]
[Flags]
public enum SecurityProof
{
	None = 0,
	Password = 1,
	Totp = 2,
	Passkey = 4,
	ExternalLogin = 8,

	PasswordAndTotp = Password | Totp,
}
