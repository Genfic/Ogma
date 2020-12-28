using System;

namespace Ogma3.Data.Models
{
    public class Document : BaseModel
    {
        public string Title { get; set; }
        public string Slug { get; set; }
        public DateTime? RevisionDate { get; set; }
        public DateTime CreationTime { get; set; }
        public uint Version { get; set; }
        public string Body { get; set; }
    }
}