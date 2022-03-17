using Ogma3.Infrastructure.PostgresEnumHelper;
using SourceGenerators.Attributes;

namespace Ogma3.Data.Clubs;

[PostgresEnum]
[PgTestAttr]
public enum EClubMemberRoles
{
    Founder = 1,
    Admin = 2,
    Moderator = 3,
    User = 4
}