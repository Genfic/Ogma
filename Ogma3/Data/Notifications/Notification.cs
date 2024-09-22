using Ogma3.Data.Bases;
using Ogma3.Data.Users;

namespace Ogma3.Data.Notifications;

public sealed class Notification : BaseModel
{
	public string? Body { get; init; }
	public string Url { get; init; } = null!;
	public DateTimeOffset DateTime { get; init; }
	public ENotificationEvent Event { get; init; }
	public ICollection<OgmaUser> Recipients { get; init; } = null!;
}