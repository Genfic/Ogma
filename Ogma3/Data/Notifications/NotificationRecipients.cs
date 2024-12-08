#nullable disable

using AutoDbSetGenerators;
using Ogma3.Data.Users;

namespace Ogma3.Data.Notifications;

[AutoDbSet]
public sealed class NotificationRecipients
{
	public OgmaUser Recipient { get; init; }
	public long RecipientId { get; init; }
	public Notification Notification { get; init; }
	public long NotificationId { get; init; }
}