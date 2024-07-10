using System;

namespace Ogma3.Data.Chapters;

public record ChapterBasic(long Id, string Slug, string Title, DateTime PublishDate, bool IsPublished, bool IsBlocked, int WordCount);