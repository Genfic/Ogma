using System.Collections.Immutable;

namespace Ogma3.Services.TimeService;

public interface ITimeService
{
	ImmutableSortedSet<TimezoneEntry> AvailableTimezones { get; }
}