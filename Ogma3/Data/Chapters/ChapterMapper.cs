using Riok.Mapperly.Abstractions;

namespace Ogma3.Data.Chapters;

[Mapper]
public static partial class ChapterMapper
{
	public static partial IQueryable<ChapterBasic> ProjectToBasic(this IQueryable<Chapter> chapters);
	
	[MapProperty(nameof(Chapter.PublicationDate), nameof(ChapterBasic.IsPublished), Use = nameof(MapIsPublished))]
	[MapProperty(nameof(Chapter.ContentBlockId), nameof(ChapterBasic.IsBlocked), Use = nameof(MapIsBlocked))]
	[MapPropertyFromSource(nameof(ChapterBasic.PublishDate), Use=nameof(MapPublishDate))]
	public static partial ChapterBasic ProjectToChapterBasic(this Chapter s);
	
	[UserMapping(Default = false)]
	private static DateTime MapPublishDate(Chapter c) => c.PublicationDate ?? c.CreationDate;
	
	[UserMapping(Default = false)]
	private static bool MapIsPublished(DateTime? s) => s != null;
	
	[UserMapping(Default = false)]
	private static bool MapIsBlocked(long? s) => s != null;
}