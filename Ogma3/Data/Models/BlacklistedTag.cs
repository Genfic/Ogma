namespace Ogma3.Data.Models
{
    public class BlacklistedTag
    {
        public OgmaUser User { get; set; }
        public long UserId { get; set; }

        public Tag Tag { get; set; }
        public long TagId { get; set; }
    }
}