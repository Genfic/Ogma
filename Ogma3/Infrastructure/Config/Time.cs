using ConfigBinder.Attributes;
using Immediate.Validations.Shared;

namespace Ogma3.Infrastructure.Config;

[Validate]
[ConfigSection("Time")]
public sealed partial class TimeOptions : IValidationTarget<TimeOptions>
{
	public required bool UseBuiltInTimezoneStyle { get; init; }
}