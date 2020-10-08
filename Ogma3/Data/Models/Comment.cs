using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Ogma3.Data.Enums;

namespace Ogma3.Data.Models
{
    public class Comment : BaseModel
    {
        [Required] 
        public long CommentsThreadId { get; set; }

        public  OgmaUser? Author { get; set; }
        public long? AuthorId { get; set; }

        [Required] 
        public DateTime DateTime { get; set; } = DateTime.Now;

        [DefaultValue(null)]
        public DateTime? LastEdit { get; set; }

        [Required]
        [MinLength(CTConfig.CComment.MinBodyLength)]
        [MaxLength(CTConfig.CComment.MaxBodyLength)]
        public string Body { get; set; }

        // Metadata about comment deletion
        [DefaultValue(null)]
        public EDeletedBy? DeletedBy { get; set; }
        
        [DefaultValue(null)]
        public OgmaUser? DeletedByUser { get; set; }
        
        [DefaultValue(null)]
        public long? DeletedByUserId { get; set; }
        
        // Metadata about edits
        public IList<CommentRevision> Revisions { get; set; }
        
        [DefaultValue(null)]
        public ushort? EditCount { get; set; }
    }
}