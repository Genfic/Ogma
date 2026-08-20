using AutoDbSetGenerators;
using Ogma3.Data.Bases;
using Ogma3.Data.CommentsThreads;
using Ogma3.Data.Users;

namespace Ogma3.Data.NewsPosts;

[AutoDbSet]
public sealed class News : BaseModel
{
	public required string Title { get; init; }
	public required string Body { get; init; }
	public required string Slug { get; init; }
	public required int ExcerptCutoff { get; init; }
	public DateTimeOffset CreationDate { get; init; }
	public DateTimeOffset? PublicationDate { get; init => field ??= value; } // immutable once set
	public bool IsVisible { get; init; }

	public OgmaUser Author { get; init; } = null!;
	public required long AuthorId { get; init; }

	public required CommentThread CommentThread { get; init; }
}