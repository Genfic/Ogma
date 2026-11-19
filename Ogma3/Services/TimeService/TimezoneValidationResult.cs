namespace Ogma3.Services.TimeService;

public sealed record InvalidTimezone;

public union TimezoneValidationResult(InvalidTimezone, TimeZoneInfo)
{
	public static InvalidTimezone Invalid
	{
		get;
	} = new();
}