using NpgSqlGenerators;

namespace Ogma3.Data.Infractions;

[PostgresEnum]
public enum InfractionType
{
	Note,
	Warning,
	Mute,
	Ban
}