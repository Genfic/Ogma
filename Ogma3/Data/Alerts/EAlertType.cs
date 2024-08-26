namespace Ogma3.Data.Alerts;

public enum EAlertType
{
	Success = 1,
	Info = 2,
	Warning = 3,
	Danger = 4,
}

public static class EAlertTypeExtensions
{
	public static string GetIcon(this EAlertType type) => type switch
	{
		EAlertType.Success => "thumb_up",
		EAlertType.Info => "info",
		EAlertType.Warning => "report_problem",
		EAlertType.Danger => "dangerous",
		_ => throw new ArgumentOutOfRangeException(nameof(type), type, null),
	};
}