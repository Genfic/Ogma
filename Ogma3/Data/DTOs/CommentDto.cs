using System;
using Ogma3.Data.Enums;

namespace Ogma3.Data.DTOs
{
    public class CommentDto
    {
        public long Id { get; set; }
        public UserSimpleDto? Author { get; set; }
        public DateTime DateTime { get; set; }
        public DateTime? LastEdit { get; set; }
        public ushort EditCount { get; set; }
        public bool Owned { get; set; }
        public string Body { get; set; }
        public EDeletedBy? DeletedBy { get; set; }
        public bool IsBlocked { get; set; }
    }
}