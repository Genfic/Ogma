namespace Ogma3.Services.TurnstileService;

public class TurnstileSettings
{
	public const string Section = nameof(TurnstileSettings);
	
	public required string Secret { get; init; }
	public required string SiteKey { get; init; }
}