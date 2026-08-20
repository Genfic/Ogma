using NetEscapades.EnumGenerators;

namespace Ogma3.Data.Comments;

[EnumExtensions]
public enum CommentSource : short
{
	Chapter,
	Blogpost,
	Profile,
	ForumPost,
	NewsPost,
}