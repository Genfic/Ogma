using System;
using System.ComponentModel.DataAnnotations;
using Ogma3.Data.Bases;

namespace Ogma3.Data.Comments
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