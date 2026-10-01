using System.Security.Claims;
using Ogma3.Data;

namespace Ogma3.Infrastructure.Extensions;

/// <summary>
/// Outcome of resolving a schedule input into the instant to publish at
/// </summary>
/// <param name="Utc">The instant to publish at, or `null` when nothing was scheduled</param>
/// <param name="Error">Why the entered value was rejected, or `null` when it was accepted</param>
public readonly record struct ScheduleResolution(DateTimeOffset? Utc, string? Error)
{
	/// <summary>
	/// Nothing to schedule
	/// </summary>
	public static readonly ScheduleResolution None = new(null, null);
}

public static class ScheduleExtensions
{
	extension(ClaimsPrincipal principal)
	{
		/// <summary>
		/// Resolve a schedule entered into a `datetime-local` input into the instant to publish at. The input
		/// carries no offset, so its components are read as a wall clock in the user's own timezone rather than
		/// converted, and the resulting instant is checked against the configured publication window.
		/// </summary>
		/// <param name="wallClock">Schedule as entered by the user, or `null` if none was</param>
		/// <returns>The accepted instant, or the reason the entered value was rejected</returns>
		public ScheduleResolution ResolveSchedule(DateTimeOffset? wallClock)
		{
			if (wallClock is not {} entered)
			{
				return ScheduleResolution.None;
			}

			var tz = principal.GetTimeZoneInfo();

			var local = DateTime.SpecifyKind(entered.DateTime, DateTimeKind.Unspecified);

			if (tz.IsInvalidTime(local))
			{
				return new(null, $"The entered time does not exist in {tz.Id} because the clocks change");
			}

			var utc = new DateTimeOffset(TimeZoneInfo.ConvertTimeToUtc(local, tz));

			var now = DateTimeOffset.UtcNow;
			var earliest = now + CTConfig.Publication.MinDelay;
			var latest = now + CTConfig.Publication.MaxDelay;

			if (utc < earliest)
			{
				return new(null, $"The schedule has to be at least {TimeZoneInfo.ConvertTime(earliest, tz):s} ({tz.Id})");
			}

			if (utc > latest)
			{
				return new(null, $"The schedule has to be no later than {TimeZoneInfo.ConvertTime(latest, tz):s} ({tz.Id})");
			}

			return new(utc, null);
		}
	}
}