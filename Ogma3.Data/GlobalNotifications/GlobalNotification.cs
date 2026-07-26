using AutoDbSetGenerators;
using Ogma3.Data.Bases;
using Ogma3.Data.Users;

namespace Ogma3.Data.GlobalNotifications;

[AutoDbSet]
public sealed class GlobalNotification : BaseModel
{
	public required string Message { get; init; }
	public DateTimeOffset CreatedAt { get; init; }
	public DateTimeOffset? ExpiresAt { get; init; }
	public DateTimeOffset? ArchivedAt { get; init; }
	public string? Color
	{
		get;
		init => field = value?.TrimStart('#').Trim().ToUpper();
	}

	public OgmaUser CreatedBy { get; init; } = null!;
	public long CreatedById { get; init; }
}