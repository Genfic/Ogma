using System.Numerics;
using System.Security.Cryptography;
using System.Text;
using Immediate.Injections.Shared;
using JetBrains.Annotations;
using MemoryPack;
using Ogma3.Infrastructure.Constants;
using Ogma3.Infrastructure.OgmaConfig;
using StackExchange.Redis;

namespace Ogma3.Services.PowService;

[RegisterTransient]
[UsedImplicitly]
public sealed class PowService(IConnectionMultiplexer redis, OgmaConfig config, ILogger<PowService> logger)
{
	public async Task<PowChallenge> IssueChallenge()
	{
		var token = Convert.ToHexString(RandomNumberGenerator.GetBytes(16));
		var challenge = new PowChallenge
		{
			Token = token,
			Difficulty = config.PowDifficulty,
			Target = ComputeTarget(config.PowDifficulty),
			IssuedAt = DateTimeOffset.UtcNow.ToUnixTimeSeconds(),
		};

		await redis.GetDatabase(GarnetDatabase.Misc).StringSetAsync(
			Key(token),
			MemoryPackSerializer.Serialize(challenge),
			TimeSpan.FromSeconds(config.PowExpirySeconds)
		);

		return challenge;
	}

	public async Task<PowVerificationResult> VerifyChallenge(string token, long nonce, string hash)
	{
		var db = redis.GetDatabase(GarnetDatabase.Misc);

		var body = await db.StringGetDeleteAsync(Key(token)); // atomic get + delete
		if (body.IsNull)
		{
			return PowVerificationResult.NotFound;
		}

		var challenge = MemoryPackSerializer.Deserialize<PowChallenge>(body);
		if (challenge is null)
		{
			logger.LogError("Failed to deserialize pow challenge");
			return PowVerificationResult.Invalid;
		}

		var nowTs = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
		if (nowTs - challenge.IssuedAt < config.PowMinimumSolveTimeSeconds)
		{
			return PowVerificationResult.TooFast;
		}

		var input = Encoding.UTF8.GetBytes($"{token}{nonce}");
		var computed = Convert.ToHexString(SHA256.HashData(input)).ToLower();

		var meetsTarget = MeetsTarget(Convert.FromHexString(computed), Convert.FromHexString(challenge.Target));

		return computed == hash && meetsTarget ? PowVerificationResult.Ok : PowVerificationResult.Invalid;

	}

	private sealed record TargetCache(int Difficulty, string Target);
	private static TargetCache _challengeCache = new(0, "");
	private static string ComputeTarget(int difficulty)
	{
		var cache = Volatile.Read(ref _challengeCache);
		if (difficulty == cache.Difficulty)
		{
			return cache.Target;
		}

		var max = (BigInteger.One << 256) - 1;
		var target = (max >> difficulty).ToString("x").PadLeft(64, '0');

		Interlocked.Exchange(ref _challengeCache, new(difficulty, target));

		return target;
	}

	private static bool MeetsTarget(byte[] hash, byte[] target)
	{
		if (hash.Length != target.Length)
		{
			return false;
		}

		for (var i = 0; i < hash.Length; i++)
		{
			if (hash[i] < target[i])
			{
				return true;
			}
			if (hash[i] > target[i])
			{
				return false;
			}
		}

		return true;
	}

	private static string Key(string token) => $"pow:challenge:{token}";
}
