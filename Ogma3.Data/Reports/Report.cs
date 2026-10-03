using AutoDbSetGenerators;
using Ogma3.Data.Bases;
using Ogma3.Data.Blogposts;
using Ogma3.Data.Chapters;
using Ogma3.Data.Clubs;
using Ogma3.Data.Comments;
using Ogma3.Data.Stories;
using Ogma3.Data.Users;

namespace Ogma3.Data.Reports;

[AutoDbSet]
public sealed class Report : BaseModel
{
	public OgmaUser Reporter { get; set; } = null!;
	public required long ReporterId { get; set; }
	public DateTimeOffset ReportDate { get; set; }
	public required string Reason { get; set; }
	public ReportStatus Status { get; set; } = ReportStatus.Open;

	// Blockable content
	public ReportableContentType ContentType { get; } = 0;
	public long ContentId { get; set; }

	public Comment? Comment { get; set; }
	public long? CommentId { get; set; }

	public OgmaUser? User { get; set; }
	public long? UserId { get; set; }

	public Story? Story { get; set; }
	public long? StoryId { get; set; }

	public Chapter? Chapter { get; set; }
	public long? ChapterId { get; set; }

	public Blogpost? Blogpost { get; set; }
	public long? BlogpostId { get; set; }

	public Club? Club { get; set; }
	public long? ClubId { get; set; }
}