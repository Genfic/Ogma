using Immediate.Injections.Shared;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;
using Ogma3.Data;

namespace Ogma3.Services.InviteCodeService;

/// <summary>
/// Single point of truth for redeeming an invite code. Registration is invite-only, so every
/// path that can create a user must reserve a code through here.
/// </summary>
/// <remarks>
/// A code moves through two states:
/// <list type="bullet">
/// <item><b>Reserved</b> — <c>UsedDate</c> is set but <c>UsedById</c> is null. The code is taken and
/// no longer redeemable by anyone else, but no user is attached to it yet.</item>
/// <item><b>Claimed</b> — <c>UsedById</c> is set. The code is redeemed by a specific user.</item>
/// </list>
/// Because a code can only ever have one reservation, a row with <c>UsedById == null</c> is
/// unambiguously <i>this</i> caller's reservation. That is what makes <see cref="ReleaseAsync" />
/// safe: it can only ever undo the caller's own in-flight reservation, never a completed claim
/// belonging to another registration.
/// <para>
/// The intended order is reserve → create the user → attach, so no user is ever created only to be
/// thrown away, and the window in which a crash can leave a user with no code consumed is a code
/// being burned rather than a free account being created.
/// </para>
/// </remarks>
[RegisterTransient]
[UsedImplicitly]
public sealed class InviteCodeService(AppDbContext context, ILogger<InviteCodeService> logger)
{
	/// <summary>
	/// Atomically reserves an unused invite code. Call this <b>before</b> creating a user.
	/// </summary>
	/// <remarks>
	/// The reservation is a single conditional <c>UPDATE ... WHERE Code = @code AND UsedDate IS NULL</c>,
	/// so the database — not a prior read — decides the winner when two requests race with the same
	/// code. Under PostgreSQL <c>READ COMMITTED</c> the loser's <c>UPDATE</c> blocks on the row lock
	/// and then re-evaluates the predicate, so exactly one caller can ever see
	/// <see cref="InviteCodeReservationResult.Reserved" />.
	/// <para>
	/// If the user then cannot be created, call <see cref="ReleaseAsync" /> to hand the code back.
	/// </para>
	/// </remarks>
	public async Task<InviteCodeReservationResult> ReserveAsync(string? code, CancellationToken cancellationToken)
	{
		if (string.IsNullOrWhiteSpace(code)) return InviteCodeReservationResult.NotFound;

		var reserved = await context.InviteCodes
			.Where(ic => ic.Code == code)
			.Where(ic => ic.UsedDate == null)
			.ExecuteUpdateAsync(s => s
					.SetProperty(ic => ic.UsedDate, DateTimeOffset.UtcNow),
				cancellationToken);

		if (reserved > 0) return InviteCodeReservationResult.Reserved;

		// The conditional update matched nothing. Distinguish "no such code" from "already taken"
		// so the caller can show the right message. This only runs on the failure path.
		var usedDate = await context.InviteCodes
			.Where(ic => ic.Code == code)
			.Select(ic => ic.UsedDate)
			.FirstOrDefaultAsync(cancellationToken);

		if (usedDate is null)
		{
			logger.LogWarning("An unknown invite code was submitted for redemption");
			return InviteCodeReservationResult.NotFound;
		}

		return InviteCodeReservationResult.AlreadyClaimed;
	}

	/// <summary>
	/// Attaches a successfully created user to a reservation made by <see cref="ReserveAsync" />.
	/// </summary>
	/// <remarks>
	/// The predicate is <c>UsedById == null</c>, which is only ever true for the caller's own
	/// reservation, so this cannot be steered onto another registration's code. Returns
	/// <see langword="false" /> if the reservation was lost or already attached, in which case the
	/// caller must treat the user it created as unregistered.
	/// </remarks>
	public async Task<bool> AttachAsync(string? code, long userId, CancellationToken cancellationToken)
	{
		if (string.IsNullOrWhiteSpace(code)) return false;

		var attached = await context.InviteCodes
			.Where(ic => ic.Code == code)
			.Where(ic => ic.UsedById == null)
			.ExecuteUpdateAsync(s => s
					.SetProperty(ic => ic.UsedById, userId),
				cancellationToken);

		if (attached > 0) return true;

		logger.LogError("Could not attach user {UserId} to their reserved invite code", userId);
		return false;
	}

	/// <summary>
	/// Hands a reservation made by <see cref="ReserveAsync" /> back to the unused pool.
	/// </summary>
	/// <remarks>
	/// Only for when the user could not be created, so a transient failure does not permanently
	/// burn a legitimate code. Does nothing once the code has been attached to a user, so it is
	/// safe to call defensively.
	/// </remarks>
	/// <returns><see langword="true" /> if a reservation was released.</returns>
	public async Task<bool> ReleaseAsync(string? code, CancellationToken cancellationToken)
	{
		if (string.IsNullOrWhiteSpace(code)) return false;

		var released = await context.InviteCodes
			.Where(ic => ic.Code == code)
			.Where(ic => ic.UsedById == null)
			.ExecuteUpdateAsync(s => s
					.SetProperty(ic => ic.UsedDate, (DateTimeOffset?)null),
				cancellationToken);

		return released > 0;
	}
}
