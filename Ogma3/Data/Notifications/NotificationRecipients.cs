using Ogma3.Data.Users;

namespace Ogma3.Data.Notifications
{
    public class NotificationRecipients
    {
        public OgmaUser Recipient { get; set; }
        public long RecipientId { get; set; }

        public Notification Notification { get; set; }
        public long NotificationId { get; set; }
    }
}