using Ogma3.Data.Blacklists;

namespace Ogma3.Data;

public interface IBlockableContent
{
	ContentBlock? ContentBlock { get; set; }
	long? ContentBlockId { get; set; }
}