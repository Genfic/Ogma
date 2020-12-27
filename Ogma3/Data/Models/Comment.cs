using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Ogma3.Data.Enums;

namespace Ogma3.Data.Models
{
    public class Comment : BaseModel, IReportableContent
    {
        public long CommentsThreadId { get; set; }

        public  OgmaUser? Author { get; set; }
        public long? AuthorId { get; set; }

        public DateTime DateTime { get; set; }
        public DateTime? LastEdit { get; set; }

        [MinLength(CTConfig.CComment.MinBodyLength)]
        public string Body { get; set; }

        // Metadata about comment deletion
        public EDeletedBy? DeletedBy { get; set; }
        
        public OgmaUser? DeletedByUser { get; set; }
        public long? DeletedByUserId { get; set; }
        
        // Metadata about edits
        public IList<CommentRevision> Revisions { get; set; }
        public ushort EditCount { get; set; }

        public ICollection<Report> Reports { get; set; }
    }
}