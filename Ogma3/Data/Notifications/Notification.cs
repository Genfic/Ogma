using System;
using System.Collections.Generic;
using Ogma3.Data.Bases;
using Ogma3.Data.Users;
using Ogma3.Infrastructure.Extensions;

namespace Ogma3.Data.Notifications
{
    public class Notification : BaseModel
    {
        public string? Body { get; set; }
        public string Message => Event.GetMessage();
        
        public string Url { get; set; }
        public DateTime DateTime { get; set; }
        public ENotificationEvent Event { get; set; }
        
        public ICollection<OgmaUser> Recipients { get; set; }
    }
}