using NpgSqlGenerators;

namespace Ogma3.Data.Clubs;

[PostgresEnum]
public enum EClubMemberRoles
{
	Invalid = 0,
	Founder = 1,
	Admin = 2,
	Moderator = 3,
	User = 4,
}