using System.ComponentModel.DataAnnotations;

namespace Ogma3.Infrastructure.Config;

public sealed class TimeOptions
{
	public const string Section = "Time";

	[Required]
	public required bool UseBuiltInTimezoneStyle { get; init; }
}