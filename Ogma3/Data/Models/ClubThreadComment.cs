using System;
using System.ComponentModel.DataAnnotations;

namespace Ogma3.Data.Models
{
    public class ClubThreadComment : BaseModel
    {
        [Required]
        [MinLength(CTConfig.CClubThreadComment.MinBodyLength)]
        [MaxLength(CTConfig.CClubThreadComment.MaxBodyLength)]
        public string Body { get; set; }
        
        [Required]
        public OgmaUser Author { get; set; }
        
        [Required] 
        public DateTime DateTime { get; set; } = DateTime.Now;
    }
}