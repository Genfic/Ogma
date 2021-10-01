using Ogma3.Infrastructure.PostgresEnumHelper;

namespace Ogma3.Data.Clubs;

[PostgresEnum]
public enum EClubMemberRoles
{
    Founder = 1,
    Admin = 2,
    Moderator = 3,
    User = 4
}