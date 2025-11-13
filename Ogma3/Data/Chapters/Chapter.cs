using AutoDbSetGenerators;
using Ogma3.Data.Bases;
using Ogma3.Data.Blacklists;
using Ogma3.Data.CommentsThreads;
using Ogma3.Data.Reports;
using Ogma3.Data.Stories;

namespace Ogma3.Data.Chapters;

[AutoDbSet]
public sealed class Chapter : BaseModel, IBlockableContent, IReportableContent, IDateableContent
{
	public uint Order { get; set; }
	public DateTimeOffset CreationDate { get; set; }
	public DateTimeOffset? PublicationDate { get; set; }
	public string Title { get; set; } = null!;
	public string Slug { get; set; } = null!;
	public string Body { get; set; } = null!;
	public string? StartNotes { get; set; }
	public string? EndNotes { get; set; }
	public int WordCount { get; set; }
	public CommentThread CommentThread { get; set; } = null!;
	public Story Story { get; set; } = null!;
	public long StoryId { get; set; }
	public ContentBlock? ContentBlock { get; set; }
	public long? ContentBlockId { get; set; }
	public List<Report> Reports { get; set; } = null!;
}