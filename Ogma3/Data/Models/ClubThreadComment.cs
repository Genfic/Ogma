using System;
using System.ComponentModel.DataAnnotations;

namespace Ogma3.Data.Models
{
    public class ClubThreadComment : BaseModel
    {
        public string Body { get; set; }
        public User Author { get; set; }
        [Required] 
        public DateTime DateTime { get; set; } = DateTime.Now;
    }
}