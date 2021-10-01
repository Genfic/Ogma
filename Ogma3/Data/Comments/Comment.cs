using System;
using System.Collections.Generic;
using Ogma3.Data.Bases;
using Ogma3.Data.CommentsThreads;
using Ogma3.Data.Reports;
using Ogma3.Data.Users;

namespace Ogma3.Data.Comments;

public class Comment : BaseModel, IReportableContent
{
    public CommentsThread CommentsThread { get; set; }
    public long CommentsThreadId { get; set; }
    public  OgmaUser? Author { get; set; }
    public long? AuthorId { get; set; }
    public DateTime DateTime { get; set; }
    public DateTime? LastEdit { get; set; }
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