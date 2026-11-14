using System.Buffers.Text;
using System.Security.Cryptography;
using System.Text;

namespace Ogma3.Services.Security;

/// <summary>
/// RFC 6238 TOTP verification for *proposed* (staged) authenticator secrets.
///
/// Used when setting up or replacing an authenticator: the proposed secret is stored in the
/// pending security transaction, and the existing authenticator keeps working until the new
/// code is verified and the replacement is committed. Cancelling therefore never disables MFA.
/// </summary>
public static class TotpStaging
{
	public const int StepSeconds = 30;
	public const int Digits = 6;

	/// <summary>
	/// Verifies a TOTP code against a base32-encoded secret at the given time,
	/// accepting ±<paramref name="windowSteps"/> steps of clock drift.
	/// </summary>
	public static bool Verify(string base32Secret, ReadOnlySpan<char> code, DateTimeOffset timestamp, int windowSteps = 1)
	{
		if (code.Length != Digits || !long.TryParse(code, out var supplied))
		{
			return false;
		}

		if (!Base32.TryDecode(base32Secret, out var key))
		{
			return false;
		}

		var step = timestamp.ToUnixTimeSeconds() / StepSeconds;

		for (var i = -windowSteps; i <= windowSteps; i++)
		{
			if (Compute(key, step + i) == supplied)
			{
				return true;
			}
		}

		return false;
	}

	/// <summary>
	/// Generates a new base32-encoded authenticator secret (20 bytes, no padding),
	/// compatible with <c>UserManager{OgmaUser}.GetAuthenticatorKeyAsync</c> storage.
	/// </summary>
	public static string GenerateSecret() => Base32.Encode(RandomNumberGenerator.GetBytes(20));

	private static long Compute(ReadOnlySpan<byte> key, long step)
	{
		Span<byte> counter = stackalloc byte[8];
		System.Buffers.Binary.BinaryPrimitives.WriteInt64BigEndian(counter, step);

		var hash = HMACSHA1.HashData(key, counter);

		// Dynamic truncation per RFC 4226
		var offset = hash[^1] & 0x0F;
		var binary = ((hash[offset] & 0x7F) << 24)
		             | ((hash[offset + 1] & 0xFF) << 16)
		             | ((hash[offset + 2] & 0xFF) << 8)
		             | (hash[offset + 3] & 0xFF);

		return binary % 1_000_000;
	}
}

/// <summary>
/// Minimal RFC 4648 base32 (no padding) encoding/decoding for authenticator secrets.
/// </summary>
public static class Base32
{
	private const string Alphabet = "ABCDEFGHIJKLMNOPQRSTUVWXYZ234567";

	public static string Encode(ReadOnlySpan<byte> bytes)
	{
		var sb = new StringBuilder((bytes.Length * 8 + 4) / 5);
		var buffer = 0;
		var bits = 0;
		foreach (var b in bytes)
		{
			buffer = (buffer << 8) | b;
			bits += 8;
			while (bits >= 5)
			{
				sb.Append(Alphabet[(buffer >> (bits - 5)) & 31]);
				bits -= 5;
			}
		}

		if (bits > 0)
		{
			sb.Append(Alphabet[(buffer << (5 - bits)) & 31]);
		}

		return sb.ToString();
	}

	public static bool TryDecode(string encoded, out byte[] bytes)
	{
		bytes = [];
		if (encoded.Length == 0 || encoded.Any(c => !Alphabet.Contains(char.ToUpperInvariant(c))))
		{
			return false;
		}

		var outputLength = encoded.Length * 5 / 8;
		var result = new byte[outputLength];
		var buffer = 0;
		var bits = 0;
		var position = 0;

		foreach (var c in encoded)
		{
			buffer = (buffer << 5) | Alphabet.IndexOf(char.ToUpperInvariant(c));
			bits += 5;
			if (bits < 8) continue;

			result[position++] = (byte)((buffer >> (bits - 8)) & 0xFF);
			bits -= 8;
		}

		bytes = result;
		return true;
	}
}
