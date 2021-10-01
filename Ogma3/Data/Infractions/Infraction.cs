using System;
using Ogma3.Data.Bases;
using Ogma3.Data.Users;

namespace Ogma3.Data.Infractions;

public class Infraction : BaseModel
{
    public OgmaUser User { get; set; }
    public long UserId { get; set; }
    public DateTime IssueDate { get; set; }
    public DateTime ActiveUntil { get; set; }
    public DateTime? RemovedAt { get; set; }
    public string Reason { get; set; }
    public InfractionType Type { get; set; }

    public OgmaUser IssuedBy { get; set; }
    public long IssuedById { get; set; }
    public OgmaUser? RemovedBy { get; set; }
    public long? RemovedById { get; set; }
}