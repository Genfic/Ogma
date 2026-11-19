using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;

namespace Ogma3.Services.TimeService;

public interface ITimeService
{
	ImmutableArray<TimezoneEntry> AvailableTimezones { get; }
	bool TryGetValid(string? input, [NotNullWhen(true)]out TimeZoneInfo? timeZoneInfo);
}