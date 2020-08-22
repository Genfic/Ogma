namespace Ogma3.Data.Models
{
    public class ClubMember
    {
        public OgmaUser Member { get; set; }
        public long MemberId { get; set; }

        public Club Club { get; set; }
        public long ClubId { get; set; }
    }
}