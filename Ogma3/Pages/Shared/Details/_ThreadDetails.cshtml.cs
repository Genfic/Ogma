using System;
using Ogma3.Data.CommentsThreads;
using Ogma3.Data.Roles;

namespace Ogma3.Pages.Shared.Details
{
    public class ThreadDetails
    {
        public long Id { get; init; }
        public long ClubId { get; init; }
        public string Title { get; init; }
        public string Body { get; init; }
        public DateTime CreationDate { get; init; }
        public string AuthorName { get; init; }
        public long AuthorId { get; init; }
        public string AuthorAvatar { get; init; }
        public OgmaRole AuthorRole { get; init; }
        public CommentsThreadDto CommentsThread { get; init; }
    }
}