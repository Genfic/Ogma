using System;
using System.Collections.Generic;
using Ogma3.Data.Bases;
using Ogma3.Data.Users;
using Ogma3.Infrastructure.Extensions;

namespace Ogma3.Data.Notifications
{
    public class Notification : BaseModel
    {
        public string? Body { get; init; }
        public string Url { get; init; }
        public DateTime DateTime { get; init; }
        public ENotificationEvent Event { get; init; }
        public ICollection<OgmaUser> Recipients { get; init; }
        public string Message => Event switch
        {
            ENotificationEvent.System => "[SYSTEM]",
            ENotificationEvent.WatchedStoryUpdated => "The story you're watching just updated.",
            ENotificationEvent.WatchedThreadNewComment => "The comments thread you're following has a new comment.",
            ENotificationEvent.FollowedAuthorNewBlogpost => "The author you're following just wrote a new blogpost.",
            ENotificationEvent.FollowedAuthorNewStory => "The author you're following just created a new story.",
            ENotificationEvent.CommentReply => "One of your comments just got a reply.",
            _ => throw new ArgumentOutOfRangeException(nameof(Event), Event, null)
        };
    }
}