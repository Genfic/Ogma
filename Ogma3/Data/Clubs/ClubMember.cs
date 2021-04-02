using System;
using Ogma3.Data.Users;

namespace Ogma3.Data.Clubs
{
    public class ClubMember
    {
        public OgmaUser Member { get; set; }
        public long MemberId { get; set; }
        public Club Club { get; set; }
        public long ClubId { get; set; }

        public EClubMemberRoles Role { get; set; }
        public DateTime MemberSince { get; set; }
    }
}