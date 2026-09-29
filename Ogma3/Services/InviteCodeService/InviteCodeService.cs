using Immediate.Injections.Shared;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;
using Ogma3.Data;

namespace Ogma3.Services.InviteCodeService;

/// <summary>
/// Single point of truth for redeeming an invite code. Registration is invite-only, so every
/// path that can create a user must claim a code through here.
/// </summary>
/// <remarks>
/// A code is unused exactly while both <c>UsedDate</c> and <c>UsedById</c> are null, and redeemed
/// once <c>UsedById</c> points at the user it authorised. There is deliberately no in-between
/// state: claiming and creating the user happen in the same transaction, so a code can never be
/// burnt on a user that does not exist and a user can never exist without a code behind it.
/// </remarks>
[RegisterTransient]
[UsedImplicitly]
public sealed class InviteCodeService(AppDbContext context, ILogger<InviteCodeService> logger)
{
	/// <summary>
	/// Atomically claims an unused invite code for a user that has already been created.
	/// </summary>
	/// <remarks>
	/// This runs inside the caller's transaction, so rolling that transaction back hands the code
	/// back without any compensating write. The claim is a single conditional
	/// <c>UPDATE ... WHERE Code = @code AND UsedById IS NULL AND UsedDate IS NULL</c>, so the
	/// database — not a prior read — decides the winner when two requests race with the same code.
	/// Under PostgreSQL <c>READ COMMITTED</c> the loser's <c>UPDATE</c> blocks on the row lock and
	/// then re-evaluates the predicate, so exactly one caller can ever see
	/// <see cref="InviteCodeRedemptionResult.Redeemed" />.
	/// </remarks>
	public async Task<InviteCodeRedemptionResult> RedeemAsync(string? code, long userId, CancellationToken cancellationToken)
	{
		if (string.IsNullOrWhiteSpace(code)) return InviteCodeRedemptionResult.NotFound;

		var now = DateTimeOffset.UtcNow;
		var redeemed = await context.InviteCodes
			.Where(ic => ic.Code == code)
			.Where(ic => ic.UsedById == null)
			.Where(ic => ic.UsedDate == null)
			.ExecuteUpdateAsync(s => s
					.SetProperty(ic => ic.UsedById, userId)
					.SetProperty(ic => ic.UsedDate, now),
				cancellationToken);

		if (redeemed > 0) return InviteCodeRedemptionResult.Redeemed;

		// The conditional update matched nothing. Distinguish "no such code" from "already taken"
		// so the caller can show the right message. This only runs on the failure path, and
		// projecting the nullable key keeps a code that exists but is not yet claimed distinct
		// from a code that does not exist at all.
		var existing = await context.InviteCodes
			.Where(ic => ic.Code == code)
			.Select(ic => ic.UsedById)
			.ToListAsync(cancellationToken);

		if (existing.Count == 0)
		{
			logger.LogWarning("An unknown invite code was submitted for redemption");
			return InviteCodeRedemptionResult.NotFound;
		}

		logger.LogWarning("Invite code was already claimed when user {UserId} tried to redeem it", userId);
		return InviteCodeRedemptionResult.AlreadyClaimed;
	}
}
