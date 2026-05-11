using ConfigBoundNET;

namespace Ogma3.Infrastructure.Config;

[ConfigSection("Time")]
public sealed partial class TimeOptions
{
	public required bool UseBuiltInTimezoneStyle { get; init; }
}