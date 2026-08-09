using System.Collections.Immutable;

namespace Ogma3.Services.TimeService;

public interface ITimeService
{
	ImmutableArray<TimezoneEntry> AvailableTimezones { get; }
}