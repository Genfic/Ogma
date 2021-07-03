using System;
using Ogma3.Data.Bases;
using Ogma3.Data.Users;

namespace Ogma3.Data.ModeratorActions
{
    public class ModeratorAction : BaseModel
    {
        public OgmaUser StaffMember { get; init; }
        public long StaffMemberId { get; init; }
        public string Description { get; init; }
        public DateTime DateTime { get; init; }
        
    }
}