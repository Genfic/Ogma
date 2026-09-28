namespace Ogma3.Services.InviteCodeService;

/// <summary>
/// Outcome of an attempt to reserve an invite code.
/// </summary>
public enum InviteCodeReservationResult
{
	/// <summary>The code is unused and has now been reserved for the caller.</summary>
	Reserved,

	/// <summary>No invite code matches the supplied value.</summary>
	NotFound,

	/// <summary>The code has already been reserved or claimed by another registration.</summary>
	AlreadyClaimed,
}
