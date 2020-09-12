using System;
using System.ComponentModel;
using Ogma3.Data.Enums;

namespace Ogma3.Data.Models
{
    public class ClubMember
    {
        public OgmaUser Member { get; set; }
        public long MemberId { get; set; }

        public Club Club { get; set; }
        public long ClubId { get; set; }

        [DefaultValue(EClubMemberRoles.User)]
        public EClubMemberRoles Role { get; set; }
        public DateTime MemberSince { get; set; }
    }
}