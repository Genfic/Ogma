using System.Security.Cryptography;
using Microsoft.AspNetCore.DataProtection;

namespace Ogma3.Services.SpeedTrapService;

public sealed class SpeedTrapService(IDataProtectionProvider dataProtection) : ISpeedTrapService
{
	private readonly IDataProtector _protector = dataProtection.CreateProtector("Register.SpeedTrap.v1");

	public string GenerateToken()
	{
		var timestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString();
		return _protector.Protect(timestamp);
	}

	public bool IsHumanSpeed(string token, int minimumSeconds)
	{
		try
		{
			var decrypted = _protector.Unprotect(token);
			if (!long.TryParse(decrypted, out var startTime))
			{
				return false;
			}

			var now = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
			return now - startTime >= minimumSeconds;
		}
		catch (CryptographicException)
		{
			return false;
		}
	}
}