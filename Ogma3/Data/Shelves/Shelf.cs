using AutoDbSetGenerators;
using Ogma3.Data.Bases;
using Ogma3.Data.Icons;
using Ogma3.Data.Stories;
using Ogma3.Data.Users;

namespace Ogma3.Data.Shelves;

[AutoDbSet]
public sealed class Shelf : BaseModel
{
	public string Name { get; set; } = null!;
	public string Description { get; set; } = null!;
	public OgmaUser Owner { get; set; } = null!;
	public long OwnerId { get; set; }
	public bool IsDefault { get; set; }
	public bool IsPublic { get; set; }
	public bool IsQuickAdd { get; set; }
	public bool TrackUpdates { get; set; }
	public string? Color { get; set; }
	public Icon? Icon { get; set; }
	public long? IconId { get; set; }

	// Stories
	public List<Story> Stories { get; set; } = null!;
}