using Immediate.Injections.Shared;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;
using Ogma3.Data;
using Sodium;

namespace Ogma3.Services.SafetyPinService;

[RegisterScoped]
[UsedImplicitly]
public sealed class SafetyPinService(AppDbContext ctx)
{
	public async Task<bool> HasPin(long uid)
	{
		return await ctx.Users
			.Where(u => u.Id == uid)
			.Select(u => u.SafetyPinHash != null || u.SafetyPinLockedOutUntil > DateTimeOffset.UtcNow)
			.FirstOrDefaultAsync();
	}

	public async Task<PinVerificationResult> VerifyPin(long uid, string pin)
	{
		var data = await ctx.Users
			.Where(u => u.Id == uid)
			.Select(u => new PinData(u.SafetyPinHash, u.SafetyPinLockedOutUntil))
			.FirstOrDefaultAsync();

		if (data is null)
		{
			return PinVerificationResult.NotFound;
		}

		if (data.Expiry > DateTimeOffset.UtcNow)
		{
			return PinVerificationResult.LockedOut;
		}

		if (data.Hash is null)
		{
			return PinVerificationResult.NoPin;
		}

		return PasswordHash.ArgonHashStringVerify(data.Hash, pin)
			? PinVerificationResult.Valid
			: PinVerificationResult.Invalid;
	}

	private sealed record PinData(string? Hash, DateTimeOffset? Expiry);
}