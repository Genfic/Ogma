using Ogma3.Data.Alerts;

namespace Ogma3.Pages.Shared;

public class Alert
{
	public required EAlertType AlertType { get; init; }
	public required string Message { get; init; }
	public required bool IsDismissible { get; init; } = true;
}