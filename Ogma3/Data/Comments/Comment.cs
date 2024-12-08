using AutoDbSetGenerators;
using Ogma3.Data.Bases;
using Ogma3.Data.CommentsThreads;
using Ogma3.Data.Reports;
using Ogma3.Data.Users;

namespace Ogma3.Data.Comments;

[AutoDbSet]
public sealed class Comment : BaseModel, IReportableContent
{
	public CommentThread CommentThread { get; set; } = null!;
	public long CommentsThreadId { get; set; }
	public OgmaUser Author { get; set; } = null!;
	public long AuthorId { get; set; }
	public DateTimeOffset DateTime { get; set; } = DateTimeOffset.UtcNow;
	public string Body { get; set; } = null!;

	// Metadata about comment deletion
	public EDeletedBy? DeletedBy { get; set; }
	public OgmaUser? DeletedByUser { get; set; }
	public long? DeletedByUserId { get; set; }
	public IList<CommentRevision> Revisions { get; set; } = null!;
	public ICollection<Report> Reports { get; set; } = null!;
}