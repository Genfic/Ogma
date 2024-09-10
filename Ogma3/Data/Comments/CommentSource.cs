using NetEscapades.EnumGenerators;

namespace Ogma3.Data.Comments;

[EnumExtensions]
public enum CommentSource
{
	Invalid = 0,
	Chapter,
	Blogpost,
	Profile,
	ForumPost,
}