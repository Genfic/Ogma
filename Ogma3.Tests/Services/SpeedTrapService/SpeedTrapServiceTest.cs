using Microsoft.AspNetCore.DataProtection;
using SpeedTrap = Ogma3.Services.SpeedTrapService.SpeedTrapService;

namespace Ogma3.Tests.Services.SpeedTrapService;

public sealed class SpeedTrapServiceTest
{
	private static SpeedTrap CreateService() => new(new EphemeralDataProtectionProvider());

	[Test]
	public async Task TestGenerateTokenReturnsNonEmptyToken()
	{
		var token = CreateService().GenerateToken();
		await Assert.That(token).IsNotEmpty();
	}

	[Test]
	public async Task TestIsHumanSpeedReturnsTrueForNewToken()
	{
		var service = CreateService();
		var token = service.GenerateToken();

		await Assert.That(service.IsHumanSpeed(token, 0)).IsTrue();
	}

	[Test]
	public async Task TestIsHumanSpeedReturnsFalseWhenMinimumNotMet()
	{
		var service = CreateService();
		var token = service.GenerateToken();

		await Assert.That(service.IsHumanSpeed(token, int.MaxValue)).IsFalse();
	}

	[Test]
	public async Task TestIsHumanSpeedWithGarbageTokenReturnsFalse()
	{
		var service = CreateService();

		await Assert.That(service.IsHumanSpeed("garbage", 0)).IsFalse();
	}

	[Test]
	public async Task TestIsHumanSpeedWithTamperedTokenReturnsFalse()
	{
		var service = CreateService();
		var token = service.GenerateToken();

		await Assert.That(service.IsHumanSpeed(token[..^1], 0)).IsFalse();
	}

	[Test]
	public async Task TestIsHumanSpeedWithNonNumericDecryptedTokenReturnsFalse()
	{
		var service = new SpeedTrap(new FakeDataProtectionProvider(new GarbageProtector()));

		await Assert.That(service.IsHumanSpeed("YW55dGhpbmc", 0)).IsFalse();
	}

	private sealed class FakeDataProtectionProvider(IDataProtector protector) : IDataProtectionProvider
	{
		public IDataProtector CreateProtector(string purpose) => protector;
	}

	private sealed class GarbageProtector : IDataProtector
	{
		public IDataProtector CreateProtector(string purpose) => this;

		public byte[] Protect(byte[] plaintext) => throw new NotImplementedException();

		public byte[] Unprotect(byte[] protectedData) => [.."not-a-long"u8];
	}
}