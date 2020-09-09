
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Ogma3.Data.Models
{
    public class ClubThread : BaseModel
    {
        [Required]
        [MinLength(CTConfig.CClubThread.MinTitleLength)]
        [MaxLength(CTConfig.CClubThread.MaxTitleLength)]
        public string Title { get; set; }
        
        [Required]
        [MinLength(CTConfig.CClubThread.MinBodyLength)]
        [MaxLength(CTConfig.CClubThread.MaxBodyLength)]
        public string Body { get; set; }
        
        [Required]
        public OgmaUser Author { get; set; }
        
        [Required] 
        public DateTime CreationDate { get; set; } = DateTime.Now;

        public CommentsThread CommentsThread { get; set; }
    }
}