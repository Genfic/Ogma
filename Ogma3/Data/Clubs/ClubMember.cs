using AutoDbSetGenerators;
using Ogma3.Data.Users;

namespace Ogma3.Data.Clubs;

[AutoDbSet]
public sealed class ClubMember
{
	public OgmaUser Member { get; init; } = null!;
	public long MemberId { get; init; }
	public Club Club { get; init; } = null!;
	public long ClubId { get; init; }
	public EClubMemberRoles Role { get; init; }
	public DateTimeOffset MemberSince { get; init; }
}