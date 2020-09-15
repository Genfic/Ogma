using System;
using System.Collections;
using System.Collections.Generic;
using Npgsql.EntityFrameworkCore.PostgreSQL.Query.ExpressionTranslators.Internal;
using Ogma3.Data.DTOs;
using Ogma3.Data.Enums;
using Ogma3.Data.Models;

namespace Ogma3.Pages.Shared
{
    public class StoryCard
    {
        public long Id { get; set; }
        public string AuthorUserName { get; set; }
        public string Title { get; set; }
        public string Slug { get; set; }
        public string Hook { get; set; }
        public string? Cover { get; set; }
        public DateTime ReleaseDate { get; set; }
        public ICollection<TagDto> Tags { get; set; }
        public Rating Rating { get; set; }
        public EStoryStatus Status { get; set; }
        public int WordCount { get; set; }
        public int ChapterCount { get; set; }
    }
}