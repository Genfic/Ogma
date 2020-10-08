using System;
using System.ComponentModel.DataAnnotations;

namespace Ogma3.Data.Models
{
    public class CommentRevision : BaseModel
    {
        [Required]
        public DateTime EditTime { get; set; } = DateTime.Now;

        [Required]
        [MinLength(CTConfig.CComment.MinBodyLength)]
        [MaxLength(CTConfig.CComment.MaxBodyLength)]
        public string Body { get; set; }

        [Required]
        public Comment Parent { get; set; }

        public long ParentId { get; set; }
    }
}