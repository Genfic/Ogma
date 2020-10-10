using System;
using System.Collections.Generic;

namespace Ogma3.Pages.Shared
{
    public class BlogpostMinimal
    {
        public long Id { get; set; }
        public string AuthorUserName { get; set; }
        public string Title { get; set; }
        public string Slug { get; set; }
        public DateTime PublishDate { get; set; }
    }
}