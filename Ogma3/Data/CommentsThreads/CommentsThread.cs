#nullable enable
using System;
using System.Collections.Generic;
using Ogma3.Data.Bases;
using Ogma3.Data.Blogposts;
using Ogma3.Data.Chapters;
using Ogma3.Data.ClubThreads;
using Ogma3.Data.Comments;
using Ogma3.Data.Users;

namespace Ogma3.Data.CommentsThreads;

public class CommentsThread : BaseModel
{
    public IList<Comment> Comments { get; set; } = null!;

    public int CommentsCount { get; set; }
    public DateTime? LockDate { get; set; }
        
    public OgmaUser? User { get; set; } = null;
    public long? UserId { get; set; } = null;
        
    public Chapter? Chapter { get; set; } = null;
    public long? ChapterId { get; set; } = null;
        
    public Blogpost? Blogpost { get; set; } = null;
    public long? BlogpostId { get; set; } = null;
        
    public ClubThread? ClubThread { get; set; } = null;
    public long? ClubThreadId { get; set; } = null;

    public ICollection<OgmaUser> Subscribers { get; set; } = null!;
}