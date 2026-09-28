using NetEscapades.EnumGenerators;

namespace Ogma3.Data;

[EnumExtensions]
public enum ContentType : byte
{
	Story = 1,
	Chapter = 2,
	Blogpost = 3,
}