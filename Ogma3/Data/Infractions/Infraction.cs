using Ogma3.Data.Bases;
using Ogma3.Data.Users;

namespace Ogma3.Data.Infractions;

public class Infraction : BaseModel
{
	public OgmaUser User { get; init; } = null!;
	public required long UserId { get; init; }
	public DateTime IssueDate { get; init; }
	public required DateTime ActiveUntil { get; init; }
	public DateTime? RemovedAt { get; set; }
	public required string Reason { get; init; }
	public required InfractionType Type { get; init; }

	public OgmaUser IssuedBy { get; init; } = null!;
	public required long IssuedById { get; init; }
	public OgmaUser? RemovedBy { get; init; }
	public long? RemovedById { get; set; }
}