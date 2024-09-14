#nullable disable

using Ogma3.Data.Users;

namespace Ogma3.Data.Notifications;

public sealed class NotificationRecipients
{
	public OgmaUser Recipient { get; init; }
	public long RecipientId { get; init; }
	public Notification Notification { get; init; }
	public long NotificationId { get; init; }
}