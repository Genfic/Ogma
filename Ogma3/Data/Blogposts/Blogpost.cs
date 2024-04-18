using System;
using System.Collections.Generic;
using Ogma3.Data.Bases;
using Ogma3.Data.Blacklists;
using Ogma3.Data.Chapters;
using Ogma3.Data.CommentsThreads;
using Ogma3.Data.Reports;
using Ogma3.Data.Stories;
using Ogma3.Data.Users;

namespace Ogma3.Data.Blogposts;

public class Blogpost : BaseBlockableModel, IBlockableContent, IReportableContent, IDateableContent
{
	public string Title { get; set; } = null!;
	public string Slug { get; set; } = null!;
	public DateTime? PublicationDate { get; set; }
	public DateTime CreationDate { get; set; }
	public OgmaUser Author { get; set; } = null!;
	public long AuthorId { get; set; }
	public string Body { get; set; } = null!;
	public CommentsThread CommentsThread { get; set; } = null!;
	public int WordCount { get; set; }
	public string[] Hashtags { get; set; } = [];

	// Attachments
	public Story? AttachedStory { get; set; }
	public long? AttachedStoryId { get; set; }

	public Chapter? AttachedChapter { get; set; }
	public long? AttachedChapterId { get; set; }

	public override ContentBlock? ContentBlock { get; set; }
	public override long? ContentBlockId { get; set; }

	public ICollection<Report> Reports { get; set; } = null!;
}