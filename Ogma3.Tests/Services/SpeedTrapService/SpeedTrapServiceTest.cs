using Microsoft.AspNetCore.DataProtection;
using Microsoft.Extensions.Time.Testing;
using SpeedTrap = Ogma3.Services.SpeedTrapService.SpeedTrapService;

namespace Ogma3.Tests.Services.SpeedTrapService;

public sealed class SpeedTrapServiceTest
{
	private static readonly DateTimeOffset Start = new(2026, 11, 11, 12, 0, 0, TimeSpan.Zero);

	private static SpeedTrap CreateService(TimeProvider timeProvider) => new(new EphemeralDataProtectionProvider(), timeProvider);

	[Test]
	public async Task TestGenerateTokenReturnsNonEmptyToken()
	{
		var token = CreateService(new FakeTimeProvider(Start)).GenerateToken();
		await Assert.That(token).IsNotEmpty();
	}

	[Test]
	public async Task TestIsHumanSpeedReturnsTrueForNewToken()
	{
		var service = CreateService(new FakeTimeProvider(Start));
		var token = service.GenerateToken();

		await Assert.That(service.IsHumanSpeed(token, 0)).IsTrue();
	}

	[Test]
	public async Task TestIsHumanSpeedReturnsFalseWhenMinimumNotMet()
	{
		var service = CreateService(new FakeTimeProvider(Start));
		var token = service.GenerateToken();

		await Assert.That(service.IsHumanSpeed(token, int.MaxValue)).IsFalse();
	}

	[Test]
	public async Task TestIsHumanSpeedReturnsFalseBeforeTheMinimumElapsed()
	{
		var clock = new FakeTimeProvider(Start);
		var service = CreateService(clock);
		var token = service.GenerateToken();
		clock.Advance(TimeSpan.FromSeconds(9));

		await Assert.That(service.IsHumanSpeed(token, 10)).IsFalse();
	}

	[Test]
	public async Task TestIsHumanSpeedReturnsTrueWhenExactlyTheMinimumElapsed()
	{
		var clock = new FakeTimeProvider(Start);
		var service = CreateService(clock);
		var token = service.GenerateToken();
		clock.Advance(TimeSpan.FromSeconds(10));

		await Assert.That(service.IsHumanSpeed(token, 10)).IsTrue();
	}

	[Test]
	public async Task TestIsHumanSpeedWithGarbageTokenReturnsFalse()
	{
		var service = CreateService(new FakeTimeProvider(Start));

		await Assert.That(service.IsHumanSpeed("garbage", 0)).IsFalse();
	}

	[Test]
	public async Task TestIsHumanSpeedWithTamperedTokenReturnsFalse()
	{
		var service = CreateService(new FakeTimeProvider(Start));
		var token = service.GenerateToken();

		await Assert.That(service.IsHumanSpeed(token[..^1], 0)).IsFalse();
	}

	[Test]
	public async Task TestIsHumanSpeedWithNonNumericDecryptedTokenReturnsFalse()
	{
		var service = new SpeedTrap(new FakeDataProtectionProvider(new GarbageProtector()), new FakeTimeProvider(Start));

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