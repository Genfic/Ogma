using System;

namespace Ogma3.Pages.Shared.Cards
{
    public class ThreadCard
    {
        public long Id { get; set; }
        public long ClubId { get; set; }
        public string Title { get; set; }
        public DateTime CreationDate { get; set; }
        public string AuthorName { get; set; }
        public string AuthorAvatar { get; set; }
        public int CommentsCount { get; set; }
    }
}