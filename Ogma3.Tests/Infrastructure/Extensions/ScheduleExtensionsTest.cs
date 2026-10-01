using System.Security.Claims;
using Ogma3.Data;
using Ogma3.Infrastructure.Extensions;

namespace Ogma3.Tests.Infrastructure.Extensions;

public sealed class ScheduleExtensionsTest
{
	private const string Warsaw = "Europe/Warsaw";

	private static ClaimsPrincipal UserIn(string timezone)
		=> new(new ClaimsIdentity([new Claim(ClaimTypes.Timezone, timezone)]));

	/// <summary>
	/// The wall clock a `datetime-local` input would submit for <paramref name="instant"/>, carrying the
	/// offset the model binder happens to assume rather than the user's own.
	/// </summary>
	private static DateTimeOffset WallClockIn(TimeZoneInfo tz, DateTimeOffset instant)
		=> new(TimeZoneInfo.ConvertTime(instant, tz).DateTime, TimeSpan.Zero);

	[Test]
	public async Task TestResolveScheduleReturnsNothingWhenNoneWasEntered()
	{
		var result = UserIn(Warsaw).ResolveSchedule(null);

		await Assert.That(result.Utc).IsNull();
		await Assert.That(result.Error).IsNull();
	}

	[Test]
	public async Task TestResolveScheduleReadsTheComponentsInTheUsersTimezone()
	{
		var user = UserIn(Warsaw);

		// Mid-June, so the entered wall clock is clear of any clock change, and far enough ahead to sit
		// inside the publication window whenever the suite happens to run
		var wallClock = new DateTimeOffset(DateTimeOffset.UtcNow.Year + 1, 6, 15, 12, 0, 0, TimeSpan.Zero);

		var result = user.ResolveSchedule(wallClock);

		await Assert.That(result.Error).IsNull();

		var utc = result.Utc.GetValueOrDefault(DateTimeOffset.UnixEpoch);
		await Assert.That(user.ToUserTime(utc).DateTime).IsEqualTo(wallClock.DateTime);
	}

	[Test]
	public async Task TestResolveScheduleIgnoresTheOffsetTheFormWasRenderedWith()
	{
		var user = UserIn(Warsaw);
		var wallClock = new DateTimeOffset(DateTimeOffset.UtcNow.Year + 1, 6, 15, 12, 0, 0, TimeSpan.Zero);

		// The same wall clock submitted with two different offsets has to resolve to the same instant
		var asUtc = user.ResolveSchedule(wallClock);
		var shifted = new DateTimeOffset(wallClock.DateTime, TimeSpan.FromHours(-7));
		var asOffset = user.ResolveSchedule(shifted);

		await Assert.That(asOffset.Utc).IsEqualTo(asUtc.Utc);
	}

	[Test]
	public async Task TestResolveScheduleRejectsATimeSkippedByDaylightSaving()
	{
		// Warsaw jumps from 02:00 straight to 03:00 on 2026-03-29
		var entered = new DateTimeOffset(2026, 3, 29, 2, 30, 0, TimeSpan.Zero);

		var result = UserIn(Warsaw).ResolveSchedule(entered);

		await Assert.That(result.Utc).IsNull();
		await Assert.That(result.Error).Contains("does not exist");
	}

	[Test]
	public async Task TestResolveScheduleRejectsATimeBeforeTheMinimumDelay()
	{
		var tz = TimeZoneInfo.FindSystemTimeZoneById(Warsaw);
		var entered = WallClockIn(tz, DateTimeOffset.UtcNow);

		var result = UserIn(Warsaw).ResolveSchedule(entered);

		await Assert.That(result.Utc).IsNull();
		await Assert.That(result.Error).Contains("at least");
		await Assert.That(result.Error).Matches(@"\d{4}-\d{2}-\d{2}T\d{2}:\d{2}");
	}

	[Test]
	public async Task TestResolveScheduleRejectsATimeBeyondTheMaximumDelay()
	{
		var tz = TimeZoneInfo.FindSystemTimeZoneById(Warsaw);
		var beyond = DateTimeOffset.UtcNow + CTConfig.Publication.MaxDelay + TimeSpan.FromDays(1);
		var entered = WallClockIn(tz, beyond);

		var result = UserIn(Warsaw).ResolveSchedule(entered);

		await Assert.That(result.Utc).IsNull();
		await Assert.That(result.Error).Contains("no later than");
		await Assert.That(result.Error).Matches(@"\d{4}-\d{2}-\d{2}T\d{2}:\d{2}");
	}

	[Test]
	public async Task TestResolveScheduleFallsBackToUtcForAnUnknownTimezone()
	{
		var instant = DateTimeOffset.UtcNow.AddDays(30);
		var entered = new DateTimeOffset(instant.DateTime, TimeSpan.Zero);

		var result = UserIn("Not/ARealZone").ResolveSchedule(entered);

		await Assert.That(result.Error).IsNull();
		await Assert.That(result.Utc).IsEqualTo(instant);
	}
}
