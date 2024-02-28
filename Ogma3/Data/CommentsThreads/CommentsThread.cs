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
	public IList<Comment> Comments { get; set; } = [];

	public int CommentsCount { get; set; }
	public DateTime? LockDate { get; set; }

	public OgmaUser? User { get; set; }
	public long? UserId { get; set; }

	public Chapter? Chapter { get; set; }
	public long? ChapterId { get; set; }

	public Blogpost? Blogpost { get; set; }
	public long? BlogpostId { get; set; }

	public ClubThread? ClubThread { get; set; }
	public long? ClubThreadId { get; set; }

	public ICollection<OgmaUser> Subscribers { get; set; } = [];
}