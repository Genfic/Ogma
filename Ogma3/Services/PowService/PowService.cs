using System.Numerics;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using Ogma3.Infrastructure.Constants;
using Ogma3.Infrastructure.OgmaConfig;
using StackExchange.Redis;

namespace Ogma3.Services.PowService;

[RegisterTransient]
public sealed class PowService(IConnectionMultiplexer redis, OgmaConfig config)
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
			JsonSerializer.Serialize(challenge, PowChallengeContext.Default.PowChallenge),
			TimeSpan.FromSeconds(config.PowExpirySeconds)
		);

		return challenge;
	}

	public async Task<bool> VerifyChallenge(string token, long nonce, string hash)
	{
		var db = redis.GetDatabase(GarnetDatabase.Misc);

		var json = await db.StringGetDeleteAsync(Key(token)); // atomic get + delete
		if (json.IsNull)
		{
			return false;
		}

		var challenge = JsonSerializer.Deserialize(json.ToString(), PowChallengeContext.Default.PowChallenge);

		if (challenge is null)
		{
			return false;
		}

		var nowTs = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
		if (nowTs - challenge.IssuedAt < config.PowMinimumSolveTimeSeconds)
		{
			return false;
		}

		var input = Encoding.UTF8.GetBytes($"{token}{nonce}");
		var computed = Convert.ToHexString(SHA256.HashData(input)).ToLower();

		var meetsTarget = MeetsTarget(Convert.FromHexString(computed), Convert.FromHexString(challenge.Target));

		return computed == hash && meetsTarget;

	}

	private static string ComputeTarget(int difficulty)
	{
		var max = BigInteger.Pow(2, 256) - 1;
		var target = max >> difficulty;
		return target.ToString("x").PadLeft(64, '0');
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