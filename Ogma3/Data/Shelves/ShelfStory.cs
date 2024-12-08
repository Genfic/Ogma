#nullable disable

using AutoDbSetGenerators;
using Ogma3.Data.Stories;

namespace Ogma3.Data.Shelves;

[AutoDbSet]
public sealed class ShelfStory
{
	public Shelf Shelf { get; init; }
	public long ShelfId { get; init; }
	public Story Story { get; init; }
	public long StoryId { get; init; }
}