#nullable enable

using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Ogma3.Data.Models
{
    public class CommentsThread : BaseModel
    {
        public ICollection<Comment> Comments { get; set; } = new List<Comment>();

        [Required]
        [DefaultValue(0)]
        public int CommentsCount { get; set; }
        
        public OgmaUser? User { get; set; }
        public long? UserId { get; set; }
        
        public Chapter? Chapter { get; set; }
        public long? ChapterId { get; set; }
        
        public Blogpost? Blogpost { get; set; }
        public long? BlogpostId { get; set; }
        
        public ClubThread? ClubThread { get; set; }
        public long? ClubThreadId { get; set; }
    }
}