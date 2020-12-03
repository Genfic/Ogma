using System;
using System.ComponentModel.DataAnnotations;

namespace Ogma3.Data.Models
{
    public class ContentBlock : BaseModel
    {
        [Required]
        public OgmaUser Issuer { get; set; }
        [Required]
        public long IssuerId { get; set; }
        [Required]
        public string Reason { get; set; }
        [Required]
        public DateTime DateTime { get; set; } = DateTime.Now;
    }
}