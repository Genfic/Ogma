using System;

namespace Ogma3.Data.DTOs
{
    public class DocumentDto
    {
        public string Title { get; set; }
        public string Slug { get; set; }
        public string Body { get; set; }
        public uint Version { get; set; }
        public Guid GroupId { get; set; }
    }

    public class DocumentVersionDto
    {
        public long Id { get; set; }
        public string Slug { get; set; }
        public uint Version { get; set; }
        public DateTime CreationTime { get; set; }
    }
}