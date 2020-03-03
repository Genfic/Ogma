using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Ogma3.Data.Models
{
    public class CommentsThread : BaseModel
    {
        public  ICollection<Comment> Comments { get; set; } = new List<Comment>();
    }
}