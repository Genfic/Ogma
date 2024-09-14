using Ogma3.Data.Tags;
using Ogma3.Pages.Shared.Cards;
using Riok.Mapperly.Abstractions;

namespace Ogma3.Data.Stories;

[Mapper]
public static partial class StoryMapper
{
	public static partial IQueryable<StoryCard> ProjectToCard(this IQueryable<Story> q);
	
	[MapProperty(nameof(Story.Tags), nameof(StoryCard.Tags), Use = nameof(MapOrderedTags))]
	public static partial StoryCard ProjectToCard(this Story story);
	
	private static IEnumerable<TagDto> MapOrderedTags(IEnumerable<Tag> tags)
		=> tags.OrderByDescending(t => t.Namespace.HasValue).ThenByDescending(t => t.Namespace).Select(t => t.ToDto());
	
	public static partial IQueryable<StoryDetails> ProjectToStoryDetails(this IQueryable<Story> q);
	
	[MapProperty(nameof(Story.PublicationDate), nameof(StoryDetails.IsPublished), Use = nameof(MapIsPublished))]
	[MapProperty(nameof(Story.Tags), nameof(StoryDetails.Tags), Use = nameof(MapOrderedTags))]
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