namespace Ogma3.Data.Chapters;

public record ChapterBasic(long Id, string Slug, string Title, DateTimeOffset PublishDate, bool IsPublished, bool IsBlocked, int WordCount);