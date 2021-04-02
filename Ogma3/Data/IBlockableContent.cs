using Ogma3.Data.Blacklists;

namespace Ogma3.Data
{
    public interface IBlockableContent
    {
        public ContentBlock? ContentBlock { get; set; }
        public long? ContentBlockId { get; set; }
    }
}