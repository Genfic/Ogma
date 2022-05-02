using System;
using System.Security.Cryptography;
using Serilog;

namespace Ogma3.Services.CodeGenerator;

public class CodeGenerator : ICodeGenerator
{
	public string GetInviteCode(bool isBase64 = false)
	{
		var unix = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
		var unixStr = unix.ToString("0000000000")[^6..];

		var bytes = RandomNumberGenerator.GetBytes(6);

		var hexArray = Array.ConvertAll(bytes, x => x.ToString("X2"));
		var hexStr = string.Concat(hexArray);

		var resultCode = isBase64
			? Convert.ToBase64String(Convert.FromHexString(unixStr + hexStr))
			: unixStr + hexStr;

		Log.Information("Generated invite code: {Code} from {Unix} and {Hex}", resultCode, unixStr, hexStr);

		return resultCode;
	}
}