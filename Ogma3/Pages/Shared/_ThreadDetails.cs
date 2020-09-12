using System;
using Ogma3.Data.Models;

namespace Ogma3.Pages.Shared
{
    public class ThreadDetails
    {
        public long Id { get; set; }
        public long ClubId { get; set; }
        public string Title { get; set; }
        public string Body { get; set; }
        public DateTime CreationDate { get; set; }
        public string AuthorName { get; set; }
        public string AuthorAvatar { get; set; }
        public OgmaRole AuthorRole { get; set; }
        public CommentsThread CommentsThread { get; set; }
    }
}