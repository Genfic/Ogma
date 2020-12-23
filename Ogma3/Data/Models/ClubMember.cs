using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Ogma3.Data.Enums;

namespace Ogma3.Data.Models
{
    public class ClubMember
    {
        [Required]
        public OgmaUser Member { get; set; }
        public long MemberId { get; set; }

        [Required]
        public Club Club { get; set; }
        public long ClubId { get; set; }

        [Required]
        [DefaultValue(EClubMemberRoles.User)]
        public EClubMemberRoles Role { get; set; }
        
        [Required]
        public DateTime MemberSince { get; set; } = DateTime.Now;
    }
}