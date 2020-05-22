using System;
using System.ComponentModel.DataAnnotations;

namespace Ogma3.Data.Models
{
    public class Comment : BaseModel
    {
        [Required] 
        public long CommentsThreadId { get; set; }

        [Required] 
        public  User Author { get; set; }

        [Required] 
        public DateTime DateTime { get; set; } = DateTime.Now;

        [Required]
        [MinLength(CTConfig.CComment.MinBodyLength)]
        [MaxLength(CTConfig.CComment.MaxBodyLength)]
        public string Body { get; set; }
    }
}