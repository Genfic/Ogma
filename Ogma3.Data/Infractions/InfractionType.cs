using NetEscapades.EnumGenerators;
using NpgSqlGenerators;

namespace Ogma3.Data.Infractions;

[PostgresEnum]
[EnumExtensions]
public enum InfractionType
{
	Note,
	Warning,
	Mute,
	Ban,
}