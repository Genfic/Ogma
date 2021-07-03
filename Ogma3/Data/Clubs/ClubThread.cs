using System;
using System.ComponentModel.DataAnnotations;
using Ogma3.Data.Bases;
using Ogma3.Data.CommentsThreads;
using Ogma3.Data.Users;

namespace Ogma3.Data.Clubs
{
    public class ClubThread : BaseModel
    {
        public string Title { get; set; }
        public string Body { get; set; }
        public OgmaUser Author { get; init; }
        public long? AuthorId { get; init; }
        public DateTime CreationDate { get; init; }
        public CommentsThread CommentsThread { get; init; }
        public Club Club { get; init; }
        public long ClubId { get; init; }
        public DateTime? DeletedAt { get; set; }
        public bool IsPinned { get; set; }
    }
}