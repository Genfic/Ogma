using System;
using System.ComponentModel.DataAnnotations;
using Ogma3.Data.Bases;
using Ogma3.Data.Users;

namespace Ogma3.Data.Blacklists
{
    public class ContentBlock : BaseModel
    {
        [Required]
        public OgmaUser Issuer { get; set; }
        public long IssuerId { get; set; }
        
        [Required]
        public string Reason { get; set; }
        
        [Required]
        public DateTime DateTime { get; set; } = DateTime.Now;

        [Required]
        public string Type { get; set; }
    }
}