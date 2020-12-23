using System.ComponentModel.DataAnnotations;

namespace Ogma3.Data.Models
{
    public class BlacklistedTag
    {
        [Required]
        public OgmaUser User { get; set; }
        public long UserId { get; set; }

        [Required]
        public Tag Tag { get; set; }
        public long TagId { get; set; }
    }
}