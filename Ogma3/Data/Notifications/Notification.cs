using System;
using System.Collections.Generic;
using Ogma3.Data.Bases;
using Ogma3.Data.Users;

namespace Ogma3.Data.Notifications;

public class Notification : BaseModel
{
    public string? Body { get; init; }
    public string Url { get; init; }
    public DateTime DateTime { get; init; }
    public ENotificationEvent Event { get; init; }
    public ICollection<OgmaUser> Recipients { get; init; }
}