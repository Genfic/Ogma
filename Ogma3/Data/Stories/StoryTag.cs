using AutoDbSetGenerators;
using Ogma3.Data.Tags;

namespace Ogma3.Data.Stories;

[AutoDbSet]
public sealed class StoryTag
{
	public Story Story { get; set; } = null!;
	public long StoryId { get; set; }
	public Tag Tag { get; set; } = null!;
	public long TagId { get; set; }
}