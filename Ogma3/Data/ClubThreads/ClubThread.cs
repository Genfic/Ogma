#nullable disable

using System;
using Ogma3.Data.Bases;
using Ogma3.Data.Clubs;
using Ogma3.Data.CommentsThreads;
using Ogma3.Data.Users;

namespace Ogma3.Data.ClubThreads;

public class ClubThread : BaseModel
{
	public string Title { get; set; }
	public string Body { get; set; }
	public OgmaUser Author { get; init; }
	public long AuthorId { get; init; }
	public DateTime CreationDate { get; init; }
	public CommentsThread CommentsThread { get; init; }
	public Club Club { get; init; }
	public long ClubId { get; init; }
	public DateTime? DeletedAt { get; init; }
	public bool IsPinned { get; set; }
}