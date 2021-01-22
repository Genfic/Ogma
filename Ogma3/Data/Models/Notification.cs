using System;
using System.Collections.Generic;
using Ogma3.Data.Enums;

namespace Ogma3.Data.Models
{
    public class Notification : BaseModel
    {
        public string Body { get; set; }
        
        public string Url { get; set; }
        public DateTime DateTime { get; set; }
        public ENotificationEvent Event { get; set; }
        
        public ICollection<OgmaUser> Recipients { get; set; }
    }
}