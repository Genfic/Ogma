using Ogma3.Data.Blacklists;

namespace Ogma3.Data.Bases;

public abstract class BaseBlockableModel : BaseModel, IBlockableContent
{
	public abstract ContentBlock? ContentBlock { get; set; }
	public abstract long? ContentBlockId { get; set; }
}