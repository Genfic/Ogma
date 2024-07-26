using Ogma3.Pages.Shared.Cards;
using Riok.Mapperly.Abstractions;

namespace Ogma3.Data.Stories;

[Mapper]
public static partial class StoryMapper
{
	public static partial IQueryable<StoryCard> ProjectToCard(this IQueryable<Story> q);
	
	public static partial IQueryable<StoryDetails> ProjectToStoryDetails(this IQueryable<Story> q);
	
	[MapProperty(nameof(Story.PublicationDate), nameof(StoryDetails.IsPublished), Use = nameof(MapIsPublished))]
	[MapPropertyFromSource(nameof(StoryDetails.ReleaseDate), Use=nameof(MapReleaseDate))]
	[MapPropertyFromSource(nameof(StoryDetails.CommentsCount), Use=nameof(MapCommentsCount))]
	public static partial StoryDetails ProjectToStoryDetails(this Story s);
	
	[UserMapping(Default = false)]
	private static DateTime MapReleaseDate(Story s) => s.PublicationDate ?? s.CreationDate;
	
	[UserMapping(Default = false)]
	private static bool MapIsPublished(DateTime? s) => s != null;

	[UserMapping(Default = false)]
	private static int MapCommentsCount(Story s) => s.Chapters.Sum(c => c.CommentsThread.CommentsCount);
}