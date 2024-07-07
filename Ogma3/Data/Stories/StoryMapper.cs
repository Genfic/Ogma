using System.Linq;
using Ogma3.Pages.Shared.Cards;
using Riok.Mapperly.Abstractions;

namespace Ogma3.Data.Stories;

[Mapper]
public static partial class StoryMapper
{
	public static partial IQueryable<StoryCard> ProjectToCard(this IQueryable<Story> q);
}