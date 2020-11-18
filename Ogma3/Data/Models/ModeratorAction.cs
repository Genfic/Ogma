using System;
using System.ComponentModel.DataAnnotations;

namespace Ogma3.Data.Models
{
    public class ModeratorAction : BaseModel
    {
        public OgmaUser? StaffMember { get; set; }
        public long? StaffMemberId { get; set; }
        [Required]
        public string Description { get; set; }
        [Required] 
        public DateTime DateTime { get; set; } = DateTime.Now;
        
    }
}