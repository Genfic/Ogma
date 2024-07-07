using System;

namespace Ogma3.Data.Chapters;

public class ChapterBasic
{
	public required long Id { get; init; }
	public required string Slug { get; init; }
	public required string Title { get; init; }
	public required DateTime PublishDate { get; init; }
	public required bool IsPublished { get; init; }
	public required bool IsBlocked { get; init; }
	public required int WordCount { get; init; }
}