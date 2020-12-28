using System;

namespace Ogma3.Data.Models
{
    public class ModeratorAction : BaseModel
    {
        public OgmaUser StaffMember { get; set; }
        public long StaffMemberId { get; set; }
        public string Description { get; set; }
        public DateTime DateTime { get; set; }
        
    }
}