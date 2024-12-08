using AutoDbSetGenerators;
using Ogma3.Data.Bases;
using Ogma3.Data.Blacklists;
using Ogma3.Data.Chapters;
using Ogma3.Data.Folders;
using Ogma3.Data.Ratings;
using Ogma3.Data.Reports;
using Ogma3.Data.Shelves;
using Ogma3.Data.Tags;
using Ogma3.Data.Users;
using Ogma3.Data.Votes;

namespace Ogma3.Data.Stories;

[AutoDbSet]
public sealed class Story : BaseModel, IBlockableContent, IReportableContent, IDateableContent
{
	public OgmaUser Author { get; set; } = null!;
	public long AuthorId { get; set; }
	public string Title { get; set; } = null!;
	public string Slug { get; set; } = null!;
	public string Description { get; set; } = null!;
	public string Hook { get; set; } = null!;
	public string Cover { get; set; } = null!;
	public string? CoverId { get; set; }
	public DateTimeOffset CreationDate { get; set; }
	public DateTimeOffset? PublicationDate { get; set; }
	
	public ICollection<Credit> Credits { get; set; } = [];

	public IList<Chapter> Chapters { get; set; } = null!;
	public IEnumerable<Tag> Tags { get; set; } = null!;
	public ICollection<Vote> Votes { get; set; } = null!;

	// Rating
	public Rating Rating { get; set; } = null!;
	public long RatingId { get; set; }

	// Status
	public EStoryStatus Status { get; set; }

	public int WordCount { get; set; }
	public int ChapterCount { get; set; }

	// Just for relationship purposes
	public ICollection<Folder> Folders { get; set; } = [];

	public ContentBlock? ContentBlock { get; set; }
	public long? ContentBlockId { get; set; }

	public ICollection<Report> Reports { get; set; } = null!;
	public ICollection<Shelf> Shelves { get; set; } = null!;
}

public sealed record Credit(string Role, string Name, string? Link);