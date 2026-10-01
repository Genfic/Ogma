using System.Security.Claims;
using Ogma3.Infrastructure.Extensions;

namespace Ogma3.Tests.Infrastructure.Extensions;

public sealed class ClaimsPrincipalExTest
{
	private static ClaimsPrincipal UserWithClaims()
		=> new(new ClaimsIdentity(
		[
			new Claim(ClaimTypes.NameIdentifier, "42"),
			new Claim(ClaimTypes.Email, "ursula@example.com"),
			new Claim(ClaimTypes.Name, "Ursula"),
			new Claim(ClaimTypes.IsStaff, "true"),
			new Claim(ClaimTypes.Timezone, "Europe/Warsaw"),
			new Claim(ClaimTypes.Role, "Admin"),
			new Claim(ClaimTypes.Role, "Moderator"),
		]));

	private static ClaimsPrincipal UserWithoutClaims() => new(new ClaimsIdentity());

	[Test]
	public async Task TestIsUserSameAsLoggedIn()
	{
		await Assert.That(UserWithClaims().IsUserSameAsLoggedIn(42)).IsTrue();
		await Assert.That(UserWithClaims().IsUserSameAsLoggedIn(7)).IsFalse();
	}

	[Test]
	public async Task TestGetNumericId()
	{
		await Assert.That(UserWithClaims().GetNumericId()).IsEqualTo(42L);
	}

	[Test]
	public async Task TestGetNumericIdReturnsNullWhenNotLoggedIn()
	{
		await Assert.That(UserWithoutClaims().GetNumericId()).IsNull();
	}

	[Test]
	public async Task TestGetNumericIdReturnsNullWhenUnparseable()
	{
		var principal = new ClaimsPrincipal(new ClaimsIdentity([new Claim(ClaimTypes.NameIdentifier, "not-a-number")]));
		await Assert.That(principal.GetNumericId()).IsNull();
	}

	[Test]
	public async Task TestGetEmail()
	{
		await Assert.That(UserWithClaims().GetEmail()).IsEqualTo("ursula@example.com");
		await Assert.That(UserWithoutClaims().GetEmail()).IsNull();
	}

	[Test]
	public async Task TestGetUsername()
	{
		await Assert.That(UserWithClaims().GetUsername()).IsEqualTo("Ursula");
		await Assert.That(UserWithoutClaims().GetUsername()).IsNull();
	}

	[Test]
	public async Task TestIsStaff()
	{
		await Assert.That(UserWithClaims().IsStaff()).IsTrue();
		await Assert.That(UserWithoutClaims().IsStaff()).IsFalse();
	}

	[Test]
	public async Task TestIsStaffWhenClaimValueIsNotABoolean()
	{
		var principal = new ClaimsPrincipal(new ClaimsIdentity([new Claim(ClaimTypes.IsStaff, "maybe")]));
		await Assert.That(principal.IsStaff()).IsFalse();
	}

	[Test]
	public async Task TestHasAnyRole()
	{
		await Assert.That(UserWithClaims().HasAnyRole("Admin")).IsTrue();
		await Assert.That(UserWithClaims().HasAnyRole("User")).IsFalse();
	}

	[Test]
	public async Task TestHasAllRoles()
	{
		await Assert.That(UserWithClaims().HasAllRoles("Admin", "Moderator")).IsTrue();
		await Assert.That(UserWithClaims().HasAllRoles("Admin", "User")).IsFalse();
	}

	[Test]
	public async Task TestTryGetClaimFound()
	{
		var principal = UserWithClaims();
		var found = principal.TryGetClaim(ClaimTypes.Email, out var email);

		await Assert.That(found).IsTrue();
		await Assert.That(email).IsEqualTo("ursula@example.com");
	}

	[Test]
	public async Task TestTryGetClaimMissing()
	{
		var found = UserWithClaims().TryGetClaim("x:DoesNotExist", out var value);

		await Assert.That(found).IsFalse();
		await Assert.That(value).IsNull();
	}

	[Test]
	public async Task TestGetTimezone()
	{
		await Assert.That(UserWithClaims().GetTimezone()).IsEqualTo("Europe/Warsaw");
		await Assert.That(UserWithoutClaims().GetTimezone()).IsEqualTo("UTC");
	}

	[Test]
	public async Task TestGetTimeZoneInfo()
	{
		await Assert.That(UserWithClaims().GetTimeZoneInfo().Id).IsEqualTo("Europe/Warsaw");
	}

	[Test]
	public async Task TestGetTimeZoneInfoFallsBackToUtc()
	{
		var principal = new ClaimsPrincipal(new ClaimsIdentity([new Claim(ClaimTypes.Timezone, "Not/ARealZone")]));
		await Assert.That(principal.GetTimeZoneInfo()).IsEqualTo(TimeZoneInfo.Utc);
	}

	[Test]
	public async Task TestToUserTimeUsesTheWinterOffset()
	{
		// 12:00 UTC is 13:00 in Warsaw outside daylight saving
		var utc = new DateTimeOffset(2026, 1, 15, 12, 0, 0, TimeSpan.Zero);
		var local = UserWithClaims().ToUserTime(utc);

		await Assert.That(local.Offset).IsEqualTo(TimeSpan.FromHours(1));
		await Assert.That(local.DateTime).IsEqualTo(new DateTime(2026, 1, 15, 13, 0, 0));
	}

	[Test]
	public async Task TestToUserTimeUsesTheSummerOffset()
	{
		// 12:00 UTC is 14:00 in Warsaw during daylight saving
		var utc = new DateTimeOffset(2026, 7, 15, 12, 0, 0, TimeSpan.Zero);
		var local = UserWithClaims().ToUserTime(utc);

		await Assert.That(local.Offset).IsEqualTo(TimeSpan.FromHours(2));
		await Assert.That(local.DateTime).IsEqualTo(new DateTime(2026, 7, 15, 14, 0, 0));
	}

	[Test]
	public async Task TestToUserTimePreservesTheInstant()
	{
		var utc = new DateTimeOffset(2026, 7, 15, 12, 0, 0, TimeSpan.Zero);
		await Assert.That(UserWithClaims().ToUserTime(utc).UtcDateTime).IsEqualTo(utc.UtcDateTime);
	}

	[Test]
	public async Task TestToUserTimeFallsBackToUtcForAnUnknownTimezone()
	{
		var principal = new ClaimsPrincipal(new ClaimsIdentity([new Claim(ClaimTypes.Timezone, "Not/ARealZone")]));
		var utc = new DateTimeOffset(2026, 7, 15, 12, 0, 0, TimeSpan.Zero);
		var local = principal.ToUserTime(utc);

		await Assert.That(local.Offset).IsEqualTo(TimeSpan.Zero);
		await Assert.That(local.DateTime).IsEqualTo(utc.DateTime);
	}
}