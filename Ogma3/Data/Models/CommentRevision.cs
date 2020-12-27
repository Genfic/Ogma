using System;
using System.ComponentModel.DataAnnotations;

namespace Ogma3.Data.Models
{
    public class CommentRevision : BaseModel
    {
        public DateTime EditTime { get; set; }

        [MinLength(CTConfig.CComment.MinBodyLength)]
        public string Body { get; set; }

        public Comment Parent { get; set; }
        public long ParentId { get; set; }
    }
}