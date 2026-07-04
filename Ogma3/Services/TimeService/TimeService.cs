using System.Collections.Immutable;
using Immediate.Injections.Shared;
using JetBrains.Annotations;
using Microsoft.Extensions.Options;
using Ogma3.Infrastructure.Config;

namespace Ogma3.Services.TimeService;

[RegisterSingleton<ITimeService>]
[UsedImplicitly]
public sealed class TimeService : ITimeService
{
	public ImmutableSortedSet<TimezoneEntry> AvailableTimezones { get; }

	public TimeService(IOptions<TimeOptions> options)
	{
		AvailableTimezones = TimeZoneInfo.GetSystemTimeZones()
			.Select(tzi => {
				string ianaId;
				if (tzi.HasIanaId)
				{
					ianaId = tzi.Id;
				}
				else if (TimeZoneInfo.TryConvertWindowsIdToIanaId(tzi.Id, out var converted))
				{
					ianaId = converted;
				}
				else
				{
					return null;
				}

				if (options.Value.UseBuiltInTimezoneStyle)
				{
					return new TimezoneEntry(ianaId, tzi.DisplayName, tzi.BaseUtcOffset);
				}

				var offset = tzi.BaseUtcOffset;
				var sign = offset < TimeSpan.Zero ? '-' : '+';
				var abs = offset.Duration();
				var label = $"(UTC{sign}{abs.Hours:D2}:{abs.Minutes:D2}) {ianaId.Replace('_', ' ')}";

				return new TimezoneEntry(ianaId, label, offset);
			})
			.OfType<TimezoneEntry>()
			.OrderBy(i => i.Offset)
			.ThenBy(i => i.Text)
			.ToImmutableSortedSet();
	}
}