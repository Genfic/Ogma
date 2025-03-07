using NetEscapades.EnumGenerators;
using NpgSqlGenerators;

namespace Ogma3.Data.Clubs;

[PostgresEnum]
[EnumExtensions]
public enum EClubMemberRoles
{
	Founder = 1,
	Admin = 2,
	Moderator = 3,
	User = 4,
}