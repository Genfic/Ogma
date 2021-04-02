using System;
using Ogma3.Data.Bases;
using Ogma3.Data.Users;

namespace Ogma3.Data.ModeratorActions
{
    public class ModeratorAction : BaseModel
    {
        public OgmaUser StaffMember { get; set; }
        public long StaffMemberId { get; set; }
        public string Description { get; set; }
        public DateTime DateTime { get; set; }
        
    }
}