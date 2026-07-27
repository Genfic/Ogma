using Immediate.Injections.Shared;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;
using Ogma3.Data;
using Sodium;

namespace Ogma3.Services.SafetyPinService;

[RegisterScoped]
[UsedImplicitly]
public sealed class SafetyPinService(ApplicationDbContext ctx)
{
	public async Task<bool> HasPin(long uid)
	{
		return await ctx.Users
			.Where(u => u.Id == uid)
			.Select(u => u.SafetyPinHash != null)
			.FirstOrDefaultAsync();
	}

	public async Task<bool> VerifyPin(long uid, string pin)
	{
		var hash = await ctx.Users
			.Where(u => u.Id == uid)
			.Select(u => u.SafetyPinHash)
			.FirstOrDefaultAsync();

		return hash is not null && PasswordHash.ArgonHashStringVerify(hash, pin);
	}
}