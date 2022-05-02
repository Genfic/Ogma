using Ogma3.Infrastructure.PostgresEnumHelper;

namespace Ogma3.Data.Notifications;

[PostgresEnum]
public enum ENotificationEvent
{
	System, // x
	WatchedStoryUpdated, // v
	WatchedThreadNewComment, // v
	FollowedAuthorNewBlogpost, // v
	FollowedAuthorNewStory, // v
	CommentReply, // x
}