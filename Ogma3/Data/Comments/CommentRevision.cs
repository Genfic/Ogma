using System;
using Ogma3.Data.Bases;

namespace Ogma3.Data.Comments
{
    public class CommentRevision : BaseModel
    {
        public DateTime EditTime { get; init; }
        public string Body { get; init; }
        public Comment Parent { get; init; }
        public long ParentId { get; init; }
    }
}