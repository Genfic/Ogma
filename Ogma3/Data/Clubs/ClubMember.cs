using Ogma3.Data.Users;

namespace Ogma3.Data.Clubs;

public class ClubMember
{
	public OgmaUser Member { get; init; } = null!;
	public long MemberId { get; init; }
	public Club Club { get; init; } = null!;
	public long ClubId { get; init; }
	public EClubMemberRoles Role { get; init; }
	public DateTime MemberSince { get; init; }
}