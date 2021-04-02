using System;
using Ogma3.Data.Notifications;

namespace Ogma3.Infrastructure.Extensions
{
    public static class ENotificationEventExtensions
    {
        public static string GetMessage(this ENotificationEvent @event) => @event switch
        {
            ENotificationEvent.System => "[SYSTEM]",
            ENotificationEvent.WatchedStoryUpdated => "The story you're watching just updated.",
            ENotificationEvent.WatchedThreadNewComment => "The comments thread you're following has a new comment.",
            ENotificationEvent.FollowedAuthorNewBlogpost => "The author you're following just wrote a new blogpost.",
            ENotificationEvent.FollowedAuthorNewStory => "The author you're following just created a new story.",
            ENotificationEvent.CommentReply => "One of your comments just got a reply.",
            _ => throw new ArgumentOutOfRangeException(nameof(@event), @event, null)
        };
    }
}