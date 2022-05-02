using Ogma3.Infrastructure.PostgresEnumHelper;

namespace Ogma3.Data.Infractions;

[PostgresEnum]
public enum InfractionType
{
	Note,
	Warning,
	Mute,
	Ban
}