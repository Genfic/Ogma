namespace Ogma3.Services.InviteCodeService;

/// <summary>
/// Outcome of an attempt to redeem an invite code.
/// </summary>
public enum InviteCodeRedemptionResult
{
	/// <summary>The code was unused and has now been claimed for the user.</summary>
	Redeemed,

	/// <summary>No invite code matches the supplied value.</summary>
	NotFound,

	/// <summary>The code has already been claimed by another registration.</summary>
	AlreadyClaimed,
}
