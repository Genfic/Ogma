using NetEscapades.EnumGenerators;

namespace Ogma3.Data.Alerts;

[EnumExtensions]
public enum EAlertType
{
	Success = 1,
	Info = 2,
	Warning = 3,
	Danger = 4,
}

public static partial class EAlertTypeExtensions
{
	public static string GetIcon(this EAlertType type) => type switch
	{
		EAlertType.Success => "lucide:thumbs-up",
		EAlertType.Info => "lucide:info",
		EAlertType.Warning => "lucide:triangle-alert",
		EAlertType.Danger => "lucide:circle-x",
		_ => throw new ArgumentOutOfRangeException(nameof(type), type, null),
	};
}