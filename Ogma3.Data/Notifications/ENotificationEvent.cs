using NpgSqlGenerators;

namespace Ogma3.Data.Notifications;

[PostgresEnum]
public enum ENotificationEvent
{
	System,
	WatchedStoryUpdated,
	WatchedThreadNewComment,
	FollowedAuthorNewBlogpost,
	FollowedAuthorNewStory,
	CommentReply,
	NewFollower,
}