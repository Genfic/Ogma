using System.Security.Cryptography;
using SimpleBase;

namespace Ogma3.Services.CodeGenerator;

public sealed class CodeGenerator : ICodeGenerator
{
	public string GetInviteCode()
	{
		var unix = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
		var lastDigits = (int)(unix % 1_000_000);

		Span<byte> buffer = stackalloc byte[10];
		buffer[0] = (byte)(lastDigits >> 16);
		buffer[1] = (byte)(lastDigits >> 8);
		buffer[2] = (byte)lastDigits;

		RandomNumberGenerator.Fill(buffer[3..]);

		return Base32.Crockford.Encode(buffer).Trim().ToUpperInvariant();
	}
}